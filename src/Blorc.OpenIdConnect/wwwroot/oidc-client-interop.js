window.BlorcOidc = {
    Navigation: {
        IsRedirected: function() {
            return window.performance.navigation.type === 0;
        }
    },
    Client: {
        UserManager: {
            IsInitialized: function() {
                return this.userManager !== undefined;
            },
            Initialize: function (config) {
                if (this.userManager === undefined) {
                    this.userManager = new UserManager(config);

                    var self = this;
                    this.userManager.events.addAccessTokenExpiring(function() {
                        self.userManager.signinSilent({scope: config.scope, response_type: config.response_type})
                            .then(function(u) {
                                self.SetCurrentUser(u);
                            })
                            .catch(function (e) {
                                if (e.message !== "No state in response") {
                                    console.log(e);
                                }

                                self.userManager.getUser().then(function (u) {
                                    self.SetCurrentUser(u);
                                });
                            });
                    });
                }

                return true;
            },
            IsAuthenticated: function() {
                if (this.userManager === undefined) {
                    return false;
                }

                if (this.User !== undefined) {
                    return true;
                }

                var self = this;
                return new Promise((resolve, reject) => {
                    self.userManager.signinRedirectCallback().then(function(u) {
                        resolve(u !== null);
                    }).catch(function(e) {
                        if (e.message !== "No state in response") {
                            console.log(e);
                        }

                        self.userManager.getUser().then(function(u) {
                            resolve(u != null);
                        });
                    });
                });
            },
            GetUser: function () {
                if (this.userManager === undefined) {
                    return null;
                }

                if (this.User !== undefined) {
                    return this.User;
                }

                var self = this;
                return new Promise((resolve, reject) => {
                    self.userManager.getUser().then(function(u) {
                        self.SetCurrentUser(u);
                        resolve(u);
                    }).catch(function (e) {
                        if (e.message !== "No state in response") {
                            console.log(e);
                        }

                        resolve(null);
                    });
                });
            },
            SigninRedirect: function () {
                if (this.userManager === undefined) {
                    return false;
                }

                var self = this;
                return new Promise((resolve, reject) => {
                    self.userManager.signinRedirect();
                    resolve(true);
                });
            },
            SignoutRedirect: function() {
                this.User = undefined;
                if (this.userManager === undefined) {
                    return false;
                }

                var self = this;
                return new Promise((resolve, reject) => {
                    self.userManager.signoutRedirect();
                    return resolve(true);
                });
            },
            SetCurrentUser: function(u) {
                this.User = u;
            }
        }
    }
};