window.select2Component = {
  init: function (Id) {
      //Initialize Select2 Elements
      $('#' + Id).select2({ theme: "bootstrap" });
  },
  onChange: function (id, dotnetHelper, nameFunc) {
      $('#' + id).on('select2:select', function (e) {
          dotnetHelper.invokeMethodAsync(nameFunc, $('#' + id).val());
      });
  },
}

window.internalAuth = {
  onSignInCompleted: function (user) {
    window.internalAuth.dotnetAuth.invokeMethodAsync('OnSignInCompleted', user);
  },

  onSignOutCompleted: function () {
    window.internalAuth.dotnetAuth.invokeMethodAsync('OnSignOutCompleted');
  }
}

window.onAppInitialized = function (dotnetAuth) {
  window.internalAuth.dotnetAuth = dotnetAuth;

  // Your web app's Firebase configuration
  var firebaseConfig = {
    apiKey: "AIzaSyCbQ7AKM4QUF2c4cCxbyIGQnwhpW75XzLE",
    authDomain: "iron-journal.firebaseapp.com",
    databaseURL: "https://iron-journal.firebaseio.com",
    projectId: "iron-journal",
    storageBucket: "iron-journal.appspot.com",
    messagingSenderId: "504200982047",
    appId: "1:504200982047:web:20b7b61ebc53ab16912f2a",
    measurementId: "G-XFGQ91V5DF",
  };
  // Initialize Firebase
  firebase.initializeApp(firebaseConfig);
  firebase.analytics();
  var defaultAuth = firebase.auth();

  firebase.auth().onAuthStateChanged(function (user) {
    if (user) {
      // User is signed in.
      window.internalAuth.onSignInCompleted(user);
    } else {
      // No user is signed in.
      window.internalAuth.onSignOutCompleted();
    }
  });

  // Initialize the FirebaseUI Widget using Firebase.
  window.ui = new firebaseui.auth.AuthUI(defaultAuth);
}

window.doLogin = function (container) {
  ui.start(container, {
    signInSuccessUrl: '/account/login-success',
    signInOptions: [
      // List of OAuth providers supported.
      {
        provider: firebase.auth.FacebookAuthProvider.PROVIDER_ID,
        scopes: [
          'public_profile',
          'email']
      },

    ],
    // Other config options...
  });
}

window.doLogout = function () {
  firebase.auth().signOut()
    .then(() => {
      // todo: i think i want this somehwere else
      window.location.href = "/"
    });
}

window.getUser = function () {
  var user = firebase.auth().currentUser;
  return user;
}

window.getUserIdToken = function (forceRefresh, dotnetCallback) {
  firebase.auth().currentUser.getIdToken(/* forceRefresh */ forceRefresh).then(function (idToken) {
    dotnetCallback.invokeMethodAsync("onComplete", idToken);
  }).catch(function (error) {
    dotnetCallback.invokeMethodAsync("onError", error);
  });
}

window.showPrompt = function(message, defaultValue, dotnetCallback)
{
    var value = prompt(message, defaultValue);
    return value;
}