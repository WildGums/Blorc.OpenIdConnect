window.BlorcOidc = {
    Client: {
        UserManager: {
            IsInitialized: function() {
                return this.userManager !== undefined;
            },
            Initialize: function(config) {
                if (this.userManager === undefined) {
                    this.userManager = new UserManager(config);

                    var self = this;
                    this.userManager.events.addAccessTokenExpiring(function() {
                        self.userManager.signinSilent({scope: config.scope, response_type: config.response_type})
                            .then(function(u) {
                                self.SetCurrentUser(u);
                            })
                            .catch(function(e) {
                                self.userManager.getUser().then(function (u) {
                                    self.SetCurrentUser(u);
                                });
                            });
                    });
                }

                return true;
            },
            IsAuthenticated: function() {
                if(this.User !== undefined) {
                    return true;
                }

                var self = this;
                return new Promise((resolve, reject) => {
                    self.userManager.signinRedirectCallback().then(function(u) {
                        resolve(u !== null);
                    }).catch(function(e) {
                        self.userManager.getUser().then(function(u) {
                            resolve(u != null);
                        });
                    });
                });
            },
            GetUser: function () {
                if(this.User !== undefined) {
                    return this.User;
                }

                var self = this;
                return new Promise((resolve, reject) => {
                    self.userManager.getUser().then(function(u) {
                        var user = self.SetCurrentUser(u);
                        resolve(user);
                    }).catch(function(e) {
                        resolve(null);
                    });
                });
            },
            SigninRedirect: function () {
                var self = this;
                return new Promise((resolve, reject) => {
                    self.userManager.signinRedirect();
                    resolve(true);
                });
            },
            SignoutRedirect: function() {
                this.User = undefined;
                return new Promise((resolve, reject) => {
                    this.userManager.signoutRedirect();
                    return resolve(true);
                });
            },
            SetCurrentUser: function(u) {
                if (u === undefined){
                    this.User = undefined;
                } 
                else {
                    this.User = {
                        accessToken: u.access_token,
                        tokenType: u.token_type,
                        sessionState: u.session_state,
                        expiresAt: u.expires_at,
                        profile:
                        {
                            // sid: u.profile.sid, 
                            jti: u.profile.jti,
                            sub: u.profile.sub,
                            typ: u.profile.typ,
                            azp: u.profile.azp,
                            authTime: u.profile.auth_time,
                            sessionState: u.profile.session_state,
                            acr: u.profile.acr,
                            sHash: u.profile.s_hash,
                            emailVerified: u.profile.email_verified,
                            name: u.profile.name,
                            preferredUsername: u.profile.preferred_username,
                            givenName: u.profile.given_name,
                            familyName: u.profile.family_name,
                            email: u.profile.email
                        }
                    };

                    if (u.profile.roles !== undefined) {
                        if (!Array.isArray(u.profile.roles)) {
                            this.User.profile.roles = new Array();
                            this.User.profile.roles.push(u.profile.roles);
                        } else {
                            this.User.profile.roles = u.profile.roles;
                        }
                    } else {
                        this.User.profile.roles = new Array();
                    }
                }

                return this.User;
            }
        }
    }
};