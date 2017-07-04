// Ionic Starter App

// angular.module is a global place for creating, registering and retrieving Angular modules
// 'starter' is the name of this angular module example (also set in a <body> attribute in index.html)
// the 2nd parameter is an array of 'requires'
// 'starter.controllers' is found in controllers.js
angular.module('starter', ['ionic', 'ngStorage', 'starter.controllers', 'ngDisqusApi', 'starter.services'])

.constant('HOST', 'https://cordialcode.com/app/api/ourpangea/api/') /* ajjenda base url */

.run(function($ionicPlatform, $state, $rootScope, HTTP, $ionicLoading, $localStorage, $ionicPopup) {
  $ionicPlatform.ready(function() {
    // Hide the accessory bar by default (remove this to show the accessory bar above the keyboard
    // for form inputs)
    if (window.cordova && window.cordova.plugins.Keyboard) {
      cordova.plugins.Keyboard.hideKeyboardAccessoryBar(true);
      cordova.plugins.Keyboard.disableScroll(true);

    }
    if (window.StatusBar) {
      // org.apache.cordova.statusbar required
      StatusBar.styleDefault();
    }
    if ($localStorage.User) {
      $rootScope.User = $localStorage.User;
      $state.go("app.home");
    } else {
      // $state.go("app.login");
    }
  });

  /*Get comments*/
  $rootScope.getComments = function() {
    // $ionicLoading.show();
    var url = 'https://disqus.com/api/3.0/forums/listPosts.json?forum=disqus&api_key=iKimnzoRGBzKUxfZbOQyYkzPkDtwpNgHhhfMzjN04zShkPV2RGJxrbdZ2LhydSxH';
    HTTP.get(url).success(function(response) {
      // alert(JSON.stringify(response.response));
      $rootScope.commentData = response.response;
      //  $ionicLoading.hide();

    }).error(function(error, status) {
      // $ionicLoading.hide();
    });
  };

  $rootScope.searchText = '';
  $rootScope.goSearchPage = function(text) {

    $rootScope.searchText = text;
    $state.go('searchresult');

  };

  $rootScope.Search = function(e, type) {
    var text = e.currentTarget.value;
    var issearch = false;
    if (type) {
      issearch = true;
    }
    if (e.keyCode == 13) {
      issearch = true;
    }
    if (issearch) {
      if (!text) {
        $ionicPopup.alert({
          title: '',
          template: 'Please enter keyword'
        })
        return;
      }
      $rootScope.searchText = text;
      $state.go('searchresult');
    }
  };


  $rootScope.recentVideos = [1, 2, 3];

  $rootScope.goVideoDetail = function() {
    $state.go('videodetail');
  };

  $rootScope.goAudioDetail = function() {
    $state.go('audiodetail');
  };

  $rootScope.goPhotoDetail = function() {
    $state.go('photodetail');
  };

  $rootScope.showSearchBar = function() {
    $rootScope.hideNavBar = true;
  };

  var scrollPos = 0;
  $rootScope.scrollLeft = function() {
    scrollPos = scrollPos + 50;
    $("#mainTags").scrollLeft(scrollPos);
  };

  $rootScope.scrollRight = function() {
    if (scrollPos > 0) {
      scrollPos = scrollPos - 50;
      $("#mainTags").scrollLeft(scrollPos);
    }
  };

  $rootScope.hideSearchBar = function() {
    $rootScope.hideNavBar = false;
  };

  $rootScope.hideSearchBar();

  $rootScope.tagData = ['I Miss You', 'See You Later', 'Ok', 'Good Bye', 'Good Morning', 'Happy New Year', 'Good Afternoon', 'Hiiii', 'Hello'];

  $rootScope.templates = [{
    name: 'footer-popup.html',
    url: 'templates/footer-popup.html'
  }]

  $rootScope.template_footer = $rootScope.templates[0];
})

.config(function($stateProvider, $urlRouterProvider, $ionicConfigProvider, $disqusApiProvider, $httpProvider) {

  // Set our API key for Disqus
  $disqusApiProvider.setApiKey('iKimnzoRGBzKUxfZbOQyYkzPkDtwpNgHhhfMzjN04zShkPV2RGJxrbdZ2LhydSxH');

  // Set our forum name for Disqus
  $disqusApiProvider.setForumName('vidmojiform');

  $ionicConfigProvider.navBar.alignTitle("center");
  $ionicConfigProvider.backButton.text('').icon('ion-arrow-left-c').previousTitleText(false);

  $httpProvider.defaults.headers.common = {};
  $httpProvider.defaults.headers.post = {};
  $httpProvider.defaults.headers.put = {};
  $httpProvider.defaults.headers.patch = {};

  $stateProvider
    .state('app', {
      url: '/app',
      abstract: true,
      templateUrl: 'templates/menu.html',
      controller: 'AppCtrl'
    })
    .state('login', {
      cache: false,
      url: '/login',
      templateUrl: 'templates/login.html',
      controller: 'LoginCtrl'
    })

  .state('app.settings', {
    cache: false,
    url: '/settings',
    views: {
      'menuContent': {
        templateUrl: 'templates/settings.html',
        controller: 'SettingsCtrl'
      }
    }
  })

  .state('app.home', {
    url: '/home',
    views: {
      'menuContent': {
        templateUrl: 'templates/home.html',
        controller: 'HomeCtrl'
      }
    }
  })

  .state('app.video', {
    url: '/video',
    views: {
      'menuContent': {
        templateUrl: 'templates/video.html',
        controller: 'VideoCtrl'
      }
    }
  })

  .state('app.audio', {
    url: '/audio',
    views: {
      'menuContent': {
        templateUrl: 'templates/audio.html',
        controller: 'AudioCtrl'
      }
    }
  })

  .state('app.photo', {
    url: '/photo',
    views: {
      'menuContent': {
        templateUrl: 'templates/photo.html',
        controller: 'PhotoCtrl'
      }
    }
  })

  .state('app.photosbycategory', {
    url: '/photosbycategory',
    views: {
      'menuContent': {
        templateUrl: 'templates/photos-by-category.html',
        controller: 'PhotosByCategoryCtrl'
      }
    }
  })

  .state('app.videosbycategory', {
    url: '/videosbycategory',
    views: {
      'menuContent': {
        templateUrl: 'templates/videos-by-category.html',
        controller: 'VideosByCategoryCtrl'
      }
    }
  })

  .state('app.audiobycategory', {
    url: '/audiobycategory',
    views: {
      'menuContent': {
        templateUrl: 'templates/audio-by-category.html',
        controller: 'AudioByCategoryCtrl'
      }
    }
  })

  .state('videodetail', {
    cache: false,
    url: '/videodetail/:videoid',
    templateUrl: 'templates/video-detail.html',
    controller: 'VideoDetailCtrl'
  })

  .state('audiodetail', {
    cache: false,
    url: '/audiodetail/:videoid',
    templateUrl: 'templates/audio-detail.html',
    controller: 'AudioDetailCtrl'
  })

  .state('photodetail', {
    cache: false,
    url: '/photodetail',
    templateUrl: 'templates/photo-detail.html',
    controller: 'PhotoDetailCtrl'
  })

  .state('register', {
    url: '/register',
    templateUrl: 'templates/register.html',
    controller: 'RegisterCtrl'
  })

  .state('app.profile', {
    url: '/profile',
    views: {
      'menuContent': {
        templateUrl: 'templates/profile.html',
        controller: 'ProfileCtrl'
      }
    }
  })

  .state('app.upload', {
    url: '/upload',
    views: {
      'menuContent': {
        templateUrl: 'templates/upload.html',
        controller: 'UploadCtrl'
      }
    }
  })

  .state('app.privacy', {
    url: '/privacy',
    views: {
      'menuContent': {
        templateUrl: 'templates/privacyPolicy.html',
        controller: 'PrivacyCtrl'
      }
    }
  })

  .state('app.contactus', {
    url: '/contactus',
    views: {
      'menuContent': {
        templateUrl: 'templates/contactus.html',
        controller: 'ContactCtrl'
      }
    }
  })

  .state('terms', {
    url: '/terms',
    templateUrl: 'templates/termsUse.html',
    controller: 'TermsCtrl'
  })

  .state('searchresult', {
    url: '/searchresult',
    templateUrl: 'templates/search-result.html',
    controller: 'SearchResultCtrl'
  })

  .state('videolist', {
    url: '/videolist',
    templateUrl: 'templates/video-list.html',
    controller: 'VideoListCtrl'
  })

  .state('audiolist', {
    url: '/audiolist',
    templateUrl: 'templates/audio-list.html',
    controller: 'AudioListCtrl'
  })

  .state('photolist', {
    url: '/photolist',
    templateUrl: 'templates/photo-list.html',
    controller: 'PhotoListCtrl'
  });
  // if none of the above states are matched, use this as the fallback
  $urlRouterProvider.otherwise('/login');

  // if (localStorage.getItem('rememberme') == 'true') {
  //   $urlRouterProvider.otherwise('/app/settings');
  // } else {
  //   $urlRouterProvider.otherwise('/home');
  // }

  // $urlRouterProvider.otherwise('/audiodetail');
});
