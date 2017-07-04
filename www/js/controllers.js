angular.module('starter.controllers', [])

.controller('AppCtrl', function($scope, $state, $rootScope) {

  $scope.goToPage = function(pageName) {
    $state.go('app.' + pageName);
  };

  $scope.goTerms = function() {
    $state.go('terms');
  };

  $scope.signOut = function() {
    localStorage.setItem('rememberme', 'false');
    $state.go('login');
  };


  $scope.goVideosByCategory = function(categoryName) {
    $rootScope.categotyTitle = categoryName;
    $state.go('app.videosbycategory');
  };

  $scope.goAudioByCategory = function(categoryName) {
    $rootScope.categotyTitle = categoryName;
    $state.go('app.audiobycategory');
  };

  $scope.goPhotosByCategory = function(categoryName) {
    $rootScope.categotyTitle = categoryName;
    $state.go('app.photosbycategory');
  };

})

.controller('LoginCtrl', function($scope, $localStorage, $ionicSideMenuDelegate, $state, $rootScope, $ionicLoading, $ionicPopup, AccountService) {
  $scope.logindata = {};
  $rootScope.hideNavBar = false;
  $ionicSideMenuDelegate.canDragContent(false);

  $scope.logindata.isChecked = true;



  $scope.login = function() {
    $ionicLoading.show({
      template: 'Processing...'
    });
    if (!$scope.logindata.username) {
      $ionicPopup.alert({
        title: 'alert',
        template: 'Please enter username'
      })
      $ionicLoading.hide({});
      return;
    }
    if (!$scope.logindata.password) {
      $ionicPopup.alert({
        title: 'alert',
        template: 'Please enter password'
      })
      $ionicLoading.hide({});
      return;
    }

    AccountService.login($scope.logindata, function(res) {
      $ionicLoading.hide({});
      if (res && res.data && res.data.data && res.data.data.Status == "success") {
        $rootScope.User = res.data.data.Data[0];
        $localStorage.User = res.data.data.Data[0];
        $state.go('app.home');
      } else {
        $ionicPopup.alert({
          title: 'alert',
          template: res.data.message
        });
      }
    });
  }

  $scope.stayLoggedIn = function() {
    if ($scope.isChecked) {
      $scope.isChecked = false;
    } else {
      $scope.isChecked = true;
    }
  };

  $scope.goSettings = function() {
    localStorage.setItem('rememberme', $scope.isChecked);
    $state.go('app.settings');
  };

  $scope.goRegister = function() {
    $state.go('register');
  };
})

.controller('SettingsCtrl', function($scope, $ionicSideMenuDelegate, $state, $rootScope, disqusApi, AccountService) {

  var params = {
    limit: 5,
    related: 'thread'
  }



  disqusApi.get('forums', 'listPosts', params).then(function(comments) {
    $scope.comments = comments;
    console.log(comments);

  });


  $rootScope.hideNavBar = false;
  $ionicSideMenuDelegate.canDragContent(false);

  $scope.goHome = function() {
    $state.go('app.home');
  };
})

.controller('HomeCtrl', function($scope, $ionicSideMenuDelegate, $rootScope, $interval, AlbumService) {

  $rootScope.hideNavBar = false;
  $ionicSideMenuDelegate.canDragContent(false);
  $scope.recentVideos = [1, 2, 3];



  // Video
  var video = document.getElementById("mainVideo");

  $scope.currTime = video.currentTime;
  $scope.totalDur = video.duration;
  /*$scope.buff=video.buffered.end(0);*/

  $interval(function() {
    $scope.currTime = video.currentTime;
    $scope.totalDur = video.duration;
    $scope.buff = video.buffered.end(0);
  }, 1000);

  // Buttons
  var playButton = document.getElementById("play-pause");
  var muteButton = document.getElementById("mute");
  var fullScreenButton = document.getElementById("full-screen");

  // Sliders
  var seekBar = document.getElementById("seek-bar");
  var volumeBar = document.getElementById("volume-bar");



  // Event listener for the play/pause button
  playButton.addEventListener("click", function() {
    if (video.paused == true) {
      // Play the video
      video.play();

      // Update the button text to 'Pause'
      playButton.className = "ion-pause color-white font-size-20";
    } else {
      // Pause the video
      video.pause();

      // Update the button text to 'Play'
      playButton.className = "ion-play color-white font-size-20";
    }
  });

  // Event listener for the mute button
  muteButton.addEventListener("click", function() {
    if (video.muted == false) {
      // Mute the video
      video.muted = true;

      // Update the button text
      muteButton.className = "ion-volume-mute color-white font-size-20";
    } else {
      // Unmute the video
      video.muted = false;

      // Update the button text
      muteButton.className = "ion-volume-high color-white font-size-20";
    }
  });

  // Event listener for the full-screen button
  /*   fullScreenButton.addEventListener("click", function() {
       if (video.requestFullscreen) {
         video.requestFullscreen();
       } else if (video.mozRequestFullScreen) {
         video.mozRequestFullScreen(); // Firefox
       } else if (video.webkitRequestFullscreen) {
         video.webkitRequestFullscreen(); // Chrome and Safari
       }
     });*/


  // Event listener for the seek bar
  seekBar.addEventListener("change", function() {
    // Calculate the new time
    var time = video.duration * (seekBar.value / 100);

    // Update the video time
    video.currentTime = time;
  });

  // Update the seek bar as the video plays
  video.addEventListener("timeupdate", function() {
    // Calculate the slider value
    var value = (100 / video.duration) * video.currentTime;

    // Update the slider value
    seekBar.value = value;
  });


  // Pause the video when the slider handle is being dragged
  seekBar.addEventListener("mousedown", function() {
    video.pause();
  });

  // Play the video when the slider handle is dropped
  seekBar.addEventListener("mouseup", function() {
    video.play();
  });

  // Event listener for the volume bar
  volumeBar.addEventListener("change", function() {
    // Update the video volume
    video.volume = volumeBar.value;
  });

  var loadFeaturedVideo = function() {
    var query = {
      // UserName: $rootScope.User.UserName,
      isEnabled: 1,
      isReviewed: 1,
      LoadLibrary: false,
      LoadFavorites: false, // make it true for loading favorited videos
      LoadLiked: true, // make it true for loading liked videos
      Type: 0, // 0: for videos, 1: for audio
      PageNumber: 1,
      PageSize: 10,
      Term: '', // search term if any
      Categories: '', // filter records by category
      Tags: '', // filter records by tags
      isFeatured: 1, // filter records by featured, 1: for featured, 0: for normal,
    }

    AlbumService.loadVideo(query, function(res) {

      $scope.FeaturedVideo = res.data.data.Data[0];
    });

  }

  var loadRecentVideo = function() {
    var query = {
      // UserName: $rootScope.User.UserName,
      isEnabled: 1,
      isReviewed: 1,
      LoadLibrary: false,
      LoadFavorites: false, // make it true for loading favorited videos
      LoadLiked: false, // make it true for loading liked videos
      Type: 0, // 0: for videos, 1: for audio
      PageNumber: 1,
      PageSize: 10,
      Term: '', // search term if any
      Categories: '', // filter records by category
      Tags: '', // filter records by tags
      isFeatured: 2, // filter records by featured, 1: for featured, 0: for normal,
      Order: 'date_added desc'
    }

    AlbumService.loadVideo(query, function(res) {

      $scope.recentVideos = res.data.data.Data;
    });

  }
  var loadRecentAudio = function() {
    var query = {
      // UserName: $rootScope.User.UserName,
      isEnabled: 1,
      isReviewed: 1,
      LoadLibrary: false,
      LoadFavorites: false, // make it true for loading favorited videos
      LoadLiked: false, // make it true for loading liked videos
      Type: 1, // 0: for videos, 1: for audio
      PageNumber: 1,
      PageSize: 10,
      Term: '', // search term if any
      Categories: '', // filter records by category
      Tags: '', // filter records by tags
      isFeatured: 2, // filter records by featured, 1: for featured, 0: for normal,
      Order: 'date_added desc'
    }

    AlbumService.loadVideo(query, function(res) {
      $scope.recentAudios = res.data.data.Data;
    });

  }
  var loadRecentPhoto = function() {
    var query = {
      isDisabled: 1,
      isApproved: 1,
      LoadLibrary: false,
      LoadFavorites: false, // make it true for loading favorited videos
      LoadLiked: false, // make it true for loading liked videos
      PageNumber: 1,
      PageSize: 10,
      Term: '', // search term if any
      Categories: '',
      Tags: '',
      Order: "added_date desc"
    }

    AlbumService.loadPhoto(query, function(res) {
      $scope.recentPhotos = res.data.data.Data;
    });

  }


  loadFeaturedVideo();
  loadRecentVideo();
  loadRecentAudio();
  loadRecentPhoto();
})

.controller('VideoCtrl', function($scope, $ionicSideMenuDelegate, $state, $rootScope, AlbumService) {
  $rootScope.hideNavBar = false;
  $ionicSideMenuDelegate.canDragContent(false);

  $scope.videoCategories = ['Autos and Vehicles', 'Entertainment', 'Comedy', 'Film and Animation', 'See all'];
  $scope.Archives = ['March 2016', 'February 2016', 'See All'];

  var loadvideocategory = function() {
    var query = {
      Type: 0, // 0: for videos, 5: for photos, 7: for audio
      Records: 10,
    }

    AlbumService.loadMediaCategory(query, function(res) {

      $scope.videoCategories = res.data.Data;
    });
  }

  var loadRecentVideo = function() {
    var query = {

      isEnabled: 1,
      isReviewed: 1,
      LoadLibrary: false,
      LoadFavorites: false, // make it true for loading favorited videos
      LoadLiked: false, // make it true for loading liked videos
      Type: 0, // 0: for videos, 1: for audio
      PageNumber: 1,
      PageSize: 10,
      Term: '', // search term if any
      Categories: '', // filter records by category
      Tags: '', // filter records by tags
      isFeatured: 2, // filter records by featured, 1: for featured, 0: for normal,
      Order: 'date_added desc'
    }

    AlbumService.loadVideo(query, function(res) {

      $scope.recentVideos = res.data.data.Data;
    });

  }
  loadRecentVideo();
  loadvideocategory();

})

.controller('AudioCtrl', function($scope, $ionicSideMenuDelegate, $rootScope, $state, AlbumService) {
  $rootScope.hideNavBar = false;
  $ionicSideMenuDelegate.canDragContent(false);
  var loadAudioCategory = function() {
    var query = {
      Type: 7, // 0: for videos, 5: for photos, 7: for audio
      Records: 10,
    }

    AlbumService.loadMediaCategory(query, function(res) {

      $scope.audioCategories = res.data.Data;
    });
  }

  var loadRecentAudio = function() {
    var query = {

      isEnabled: 1,
      isReviewed: 1,
      LoadLibrary: false,
      LoadFavorites: false, // make it true for loading favorited videos
      LoadLiked: false, // make it true for loading liked videos
      Type: 1, // 0: for videos, 1: for audio
      PageNumber: 1,
      PageSize: 10,
      Term: '', // search term if any
      Categories: '', // filter records by category
      Tags: '', // filter records by tags
      isFeatured: 2, // filter records by featured, 1: for featured, 0: for normal,
      Order: 'date_added desc'
    }

    AlbumService.loadVideo(query, function(res) {
      $scope.recentAudios = res.data.data.Data;
    });

  }
  loadRecentAudio();
  loadAudioCategory();
})

.controller('PhotoCtrl', function($scope, $ionicSideMenuDelegate, $rootScope, $state, AlbumService) {
  $rootScope.hideNavBar = false;
  $ionicSideMenuDelegate.canDragContent(false);

  $rootScope.categotyTitle = 'Recently Added Photos';

  $scope.photoCategories = ['Abstract', 'Aircraft', 'Animals', 'Anime', 'See all'];
  $scope.Archives = ['March 2016', 'February 2016', 'See All'];
  $scope.recentPhotos = [1, 2, 3];

  var loadPhotoCategory = function() {
    var query = {
      Type: 5, // 0: for videos, 5: for photos, 7: for audio
      Records: 10,
    }

    AlbumService.loadMediaCategory(query, function(res) {

      $scope.photoCategories = res.data.Data;
    });
  }

  var loadRecentPhoto = function() {
    var query = {
      isDisabled: 1,
      isApproved: 1,
      LoadLibrary: false,
      LoadFavorites: false, // make it true for loading favorited videos
      LoadLiked: false, // make it true for loading liked videos
      PageNumber: 1,
      PageSize: 10,
      Term: '', // search term if any
      Categories: '',
      Tags: '',
      Order: "added_date desc"
    }

    AlbumService.loadPhoto(query, function(res) {
      $scope.recentPhotos = res.data.data.Data;
    });

  }
  loadRecentPhoto();
  loadPhotoCategory();

})

.controller('PhotosByCategoryCtrl', function($scope, $ionicSideMenuDelegate, $rootScope, AlbumService) {
  $rootScope.hideNavBar = false;
  $ionicSideMenuDelegate.canDragContent(false);
  $scope.recentPhotos = [1, 2, 3];

  $scope.cateName = $rootScope.categotyTitle;

  $scope.selectedPage = 1;

  $scope.switchTab = function(tabName) {
    $scope.selectedPage = tabName;
    switch (tabName) {
      case 1:
        $scope.cateName = $rootScope.categotyTitle + ' Photos';;
        break;
      case 2:
        $scope.cateName = $rootScope.categotyTitle + ' - Most Viewed Photos';
        break;
      case 3:
        $scope.cateName = $rootScope.categotyTitle + ' - Most Liked Photos';
        break;
      default:
        console.log("WARNING : unknown tab name")
    }
  };

  $scope.switchTab(1);
})

.controller('VideosByCategoryCtrl', function($scope, $ionicSideMenuDelegate, $rootScope) {
  $rootScope.hideNavBar = false;
  $ionicSideMenuDelegate.canDragContent(false);
  $scope.recentVideos = [1, 2, 3];

  $scope.cateName = $rootScope.categotyTitle;

  $scope.selectedPage = 1;

  $scope.switchTab = function(tabName) {
    $scope.selectedPage = tabName;
    switch (tabName) {
      case 1:
        $scope.cateName = $rootScope.categotyTitle + ' Videos';;
        break;
      case 2:
        $scope.cateName = $rootScope.categotyTitle + ' - Most Viewed Videos';
        break;
      case 3:
        $scope.cateName = $rootScope.categotyTitle + ' - Top Rated Videos';
        break;
      case 4:
        $scope.cateName = $rootScope.categotyTitle + ' - Most Commented Videos';
        break;
      default:
        console.log("WARNING : unknown tab name")
    }
  };

  $scope.switchTab(1);
})

.controller('AudioByCategoryCtrl', function($scope, $ionicSideMenuDelegate, $rootScope) {
  $rootScope.hideNavBar = false;
  $ionicSideMenuDelegate.canDragContent(false);

  $scope.recentAudio = [1, 2, 3];

  $scope.cateName = $rootScope.categotyTitle;

  $scope.selectedPage = 1;

  $scope.switchTab = function(tabName) {
    $scope.selectedPage = tabName;
    switch (tabName) {
      case 1:
        $scope.cateName = $rootScope.categotyTitle + ' Audio Files';;
        break;
      case 2:
        $scope.cateName = $rootScope.categotyTitle + ' - Most Viewed Audio Files';
        break;
      case 3:
        $scope.cateName = $rootScope.categotyTitle + ' - Top Rated Audio Files';
        break;
      case 4:
        $scope.cateName = $rootScope.categotyTitle + ' - Most Commented Audio Files';
        break;
      default:
        console.log("WARNING : unknown tab name")
    }
  };

  $scope.switchTab(1);
})

.controller('VideoDetailCtrl', function($scope, $ionicSideMenuDelegate,
  $ionicLoading, HTTP, $ionicModal, $ionicScrollDelegate, $rootScope, $state, $timeout, AlbumService, $stateParams) {

  var videoid = $stateParams.videoid;

  AlbumService.getdetailvideo(videoid, function(res) {

    $scope.VideoDetail = res.data.data.Data[0];

  });

  $rootScope.hideNavBar = false;
  $scope.showControlBar = true;

  $ionicSideMenuDelegate.canDragContent(false);

  $rootScope.getComments();

  $scope.openBrowser = function(url) {
    window.open(url, '_blank', 'location=no');
  }

  $scope.playPause = function() {
    // $state.go('videolist');
    $scope.showControlBar = true;

    $timeout(function() {
      $scope.showControlBar = false;
    }, 2000);
  };

  $ionicModal.fromTemplateUrl('templates/vidmojiPopup.html', {
    scope: $scope
  }).then(function(modal) {
    $scope.vidmojiModal = modal;
  });

  $scope.openModal = function() {
    $scope.vidmojiModal.show();
  };

  $scope.closeModal = function() {
    $scope.vidmojiModal.hide();
  };

  $scope.selectedPage = "settings";

  $scope.switchTab = function(tabName) {
    $scope.selectedPage = tabName;
    switch (tabName) {
      case 'settings':
        $ionicScrollDelegate.scrollTop();
        break;
      case 'my_library':
        $ionicScrollDelegate.scrollTop();
        break;

      default:
        console.log("WARNING : unknown tab name")
    }
  };

  $scope.videoHeight = document.getElementById('mainVideoNew').offsetHeight + 10 + 'px';

  $scope.aboutGroups = [];
  for (var i = 0; i < 1; i++) {
    $scope.aboutGroups[i] = {
      name: i,
      items: []
    };
    for (var j = 0; j < 1; j++) {
      $scope.aboutGroups[i].items.push(i + '-' + j);
    }
  }

  $scope.toggleGroup = function(group) {
    if ($scope.isGroupShown(group)) {
      $scope.shownGroup = null;
    } else {
      $scope.shownGroup = group;
    }
  };
  $scope.isGroupShown = function(group) {
    return $scope.shownGroup === group;
  };



  // Video
  var video = document.getElementById("detailVideo");

  // Buttons
  var playButton = document.getElementById("play-pause1");
  var muteButton = document.getElementById("mute1");
  var fullScreenButton = document.getElementById("full-screen1");

  // Sliders
  var seekBar = document.getElementById("seek-bar1");
  var volumeBar = document.getElementById("volume-bar1");



  // Event listener for the play/pause button
  playButton.addEventListener("click", function() {
    if (video.paused == true) {
      // Play the video
      video.play();

      /*$timeout(function(){*/
      $scope.showControlBar = false;
      /*   },1000);*/

      // Update the button text to 'Pause'
      playButton.className = "ion-pause color-white font-size-20";
    } else {
      // Pause the video
      video.pause();

      // Update the button text to 'Play'
      playButton.className = "ion-play color-white font-size-20";
    }
  });

  // Event listener for the mute button
  muteButton.addEventListener("click", function() {
    $scope.showControlBar = true;
    if (video.muted == false) {
      // Mute the video
      video.muted = true;

      // Update the button text
      muteButton.className = "ion-volume-mute color-white font-size-20";
    } else {
      // Unmute the video
      video.muted = false;

      // Update the button text
      muteButton.className = "ion-volume-high color-white font-size-20";
    }
  });

  // Event listener for the full-screen button
  fullScreenButton.addEventListener("click", function() {
    $scope.showControlBar = true;
    $state.go('videolist');
    if (video.requestFullscreen) {
      video.requestFullscreen();
    } else if (video.mozRequestFullScreen) {
      video.mozRequestFullScreen(); // Firefox
    } else if (video.webkitRequestFullscreen) {
      video.webkitRequestFullscreen(); // Chrome and Safari
    }
  });


  // Event listener for the seek bar
  seekBar.addEventListener("change", function() {
    $scope.showControlBar = true;
    // Calculate the new time
    var time = video.duration * (seekBar.value / 100);

    // Update the video time
    video.currentTime = time;
  });

  // Update the seek bar as the video plays
  video.addEventListener("timeupdate", function() {
    // Calculate the slider value
    var value = (100 / video.duration) * video.currentTime;

    // Update the slider value
    seekBar.value = value;
  });


  // Pause the video when the slider handle is being dragged
  seekBar.addEventListener("mousedown", function() {
    video.pause();
  });

  // Play the video when the slider handle is dropped
  seekBar.addEventListener("mouseup", function() {
    $scope.showControlBar = true;
    video.play();
  });

  // Event listener for the volume bar
  volumeBar.addEventListener("change", function() {
    $scope.showControlBar = true;
    // Update the video volume
    video.volume = volumeBar.value;
  });



})

.controller('AudioDetailCtrl', function($scope, $ionicSideMenuDelegate, $ionicModal, $rootScope, $state, $timeout, AlbumService, $stateParams) {
  $rootScope.hideNavBar = false;
  $scope.showControlBar = true;
  $ionicSideMenuDelegate.canDragContent(false);


  var audioid = $stateParams.videoid;

  AlbumService.getdetailaudio(audioid, function(res) {

    $scope.AudioDetail = res.data.data.Data[0];

  });
  $rootScope.getComments();

  $scope.openBrowser = function(url) {
    window.open(url, '_blank', 'location=no');
  }

  $scope.playPause = function() {
    // $state.go('videolist');
    $scope.showControlBar = true;

    $timeout(function() {
      $scope.showControlBar = false;
    }, 2000);
  };

  $ionicModal.fromTemplateUrl('templates/vidmojiPopup.html', {
    scope: $scope
  }).then(function(modal) {
    $scope.vidmojiModal = modal;
  });

  $scope.openModal = function() {
    $scope.vidmojiModal.show();
  };

  $scope.closeModal = function() {
    $scope.vidmojiModal.hide();
  };

  $scope.selectedPage = "settings";

  $scope.switchTab = function(tabName) {
    $scope.selectedPage = tabName;
    switch (tabName) {
      case 'settings':
        $ionicScrollDelegate.scrollTop();
        break;
      case 'my_library':
        $ionicScrollDelegate.scrollTop();
        break;

      default:
        console.log("WARNING : unknown tab name")
    }
  };

  $scope.videoHeight = document.getElementById('mainVideoNew').offsetHeight + 10 + 'px';

  $scope.aboutGroups = [];
  for (var i = 0; i < 1; i++) {
    $scope.aboutGroups[i] = {
      name: i,
      items: []
    };
    for (var j = 0; j < 1; j++) {
      $scope.aboutGroups[i].items.push(i + '-' + j);
    }
  }

  $scope.toggleGroup = function(group) {
    if ($scope.isGroupShown(group)) {
      $scope.shownGroup = null;
    } else {
      $scope.shownGroup = group;
    }
  };
  $scope.isGroupShown = function(group) {
    return $scope.shownGroup === group;
  };



  // audio
  var video = document.getElementById("detailAudio");

  // Buttons
  var playButton = document.getElementById("play-pause2");
  var muteButton = document.getElementById("mute2");
  var fullScreenButton = document.getElementById("full-screen2");

  // Sliders
  var seekBar = document.getElementById("seek-bar2");
  var volumeBar = document.getElementById("volume-bar2");



  // Event listener for the play/pause button
  playButton.addEventListener("click", function() {
    if (video.paused == true) {
      // Play the video
      video.play();

      $scope.showControlBar = false;

      // Update the button text to 'Pause'
      playButton.className = "ion-pause color-white font-size-20";
    } else {
      // Pause the video
      video.pause();

      // Update the button text to 'Play'
      playButton.className = "ion-play color-white font-size-20";
    }
  });

  // Event listener for the mute button
  muteButton.addEventListener("click", function() {
    if (video.muted == false) {
      // Mute the video
      video.muted = true;

      // Update the button text
      muteButton.className = "ion-volume-mute color-white font-size-20";
    } else {
      // Unmute the video
      video.muted = false;

      // Update the button text
      muteButton.className = "ion-volume-high color-white font-size-20";
    }
  });

  // Event listener for the full-screen button
  fullScreenButton.addEventListener("click", function() {
    $state.go('audiolist');
    if (video.requestFullscreen) {
      video.requestFullscreen();
    } else if (video.mozRequestFullScreen) {
      video.mozRequestFullScreen(); // Firefox
    } else if (video.webkitRequestFullscreen) {
      video.webkitRequestFullscreen(); // Chrome and Safari
    }
  });


  // Event listener for the seek bar
  seekBar.addEventListener("change", function() {
    // Calculate the new time
    var time = video.duration * (seekBar.value / 100);

    // Update the video time
    video.currentTime = time;
  });

  // Update the seek bar as the video plays
  video.addEventListener("timeupdate", function() {
    // Calculate the slider value
    var value = (100 / video.duration) * video.currentTime;

    // Update the slider value
    seekBar.value = value;
  });


  // Pause the video when the slider handle is being dragged
  seekBar.addEventListener("mousedown", function() {
    video.pause();
  });

  // Play the video when the slider handle is dropped
  seekBar.addEventListener("mouseup", function() {
    video.play();
  });

  // Event listener for the volume bar
  volumeBar.addEventListener("change", function() {
    // Update the video volume
    video.volume = volumeBar.value;
  });




})

.controller('PhotoDetailCtrl', function($scope, $ionicSideMenuDelegate, $ionicModal,
  $rootScope, $state, AlbumService, $stateParams) {
  $rootScope.hideNavBar = false;
  $ionicSideMenuDelegate.canDragContent(false);


  var audioid = $stateParams.videoid;

  AlbumService.getdetailphoto(audioid, function(res) {

    $scope.PhotoDetail = res.data.data.Data[0];

  });
  $rootScope.getComments();

  $scope.openBrowser = function(url) {
    window.open(url, '_blank', 'location=no');
  }

  $scope.goPhotoList = function() {
    $state.go('photolist');
  };

  $ionicModal.fromTemplateUrl('templates/vidmojiPopup.html', {
    scope: $scope
  }).then(function(modal) {
    $scope.vidmojiModal = modal;
  });

  $scope.openModal = function() {
    $scope.vidmojiModal.show();
  };

  $scope.closeModal = function() {
    $scope.vidmojiModal.hide();
  };

  $scope.selectedPage = "settings";

  $scope.switchTab = function(tabName) {
    $scope.selectedPage = tabName;
    switch (tabName) {
      case 'settings':
        $ionicScrollDelegate.scrollTop();
        break;
      case 'my_library':
        $ionicScrollDelegate.scrollTop();
        break;

      default:
        console.log("WARNING : unknown tab name")
    }
  };

  $scope.videoHeight = document.getElementById('mainVideoNew').offsetHeight + 10 + 'px';

  $scope.aboutGroups = [];
  for (var i = 0; i < 1; i++) {
    $scope.aboutGroups[i] = {
      name: i,
      items: []
    };
    for (var j = 0; j < 1; j++) {
      $scope.aboutGroups[i].items.push(i + '-' + j);
    }
  }

  $scope.toggleGroup = function(group) {
    if ($scope.isGroupShown(group)) {
      $scope.shownGroup = null;
    } else {
      $scope.shownGroup = group;
    }
  };
  $scope.isGroupShown = function(group) {
    return $scope.shownGroup === group;
  };




})

.controller('RegisterCtrl', function($scope, $ionicSideMenuDelegate, $state, $rootScope, $ionicLoading, $ionicPopup, AccountService) {
  $scope.signupdata = {};
  $scope.signupdata.gender = "1";
  $rootScope.hideNavBar = false;
  $ionicSideMenuDelegate.canDragContent(false);
  $scope.signup = function() {
    $ionicLoading.show({
      template: 'Processing...'
    });
    if (!$scope.signupdata.username) {
      $ionicPopup.alert({
        title: 'alert',
        template: 'Please enter username'
      })
      $ionicLoading.hide({});
      return;
    }
    if (!$scope.signupdata.password) {
      $ionicPopup.alert({
        title: 'alert',
        template: 'Please enter password'
      })
      $ionicLoading.hide({});
      return;
    }

    if ($scope.signupdata.password != $scope.signupdata.confirmpassword) {
      $ionicPopup.alert({
        title: 'alert',
        template: 'Password not match'
      })
      $ionicLoading.hide({});
      return;
    }

    if (!$scope.signupdata.email) {
      $ionicPopup.alert({
        title: 'alert',
        template: 'Please enter email'
      })
      $ionicLoading.hide({});
      return;
    }

    if (!$scope.signupdata.isgree) {
      $ionicPopup.alert({
        title: 'alert',
        template: 'Are you agree with my terms of service'
      })
      $ionicLoading.hide({});
      return;
    }
    var data = {};
    data.UserName = $scope.signupdata.username;
    data.Password = $scope.signupdata.password;
    data.Email = $scope.signupdata.email;
    data.Gender = $scope.signupdata.gender;
    data.Country = "VietNam";
    AccountService.signup(data, function(res) {
      if (res && res.data.status == "success") {
        $state.go('login');
      } else {
        $ionicPopup.alert({
          title: 'alert',
          template: res.data.message
        })
      }
    });
  }
  $scope.goLogin = function() {
    $state.go('login');
  };
})

.controller('ProfileCtrl', function($scope, $ionicSideMenuDelegate, $ionicScrollDelegate, $rootScope) {
  $rootScope.hideNavBar = false;
  $ionicSideMenuDelegate.canDragContent(true);

  $scope.selectedPage = "overview";

  $scope.switchTab = function(tabName) {
    $scope.selectedPage = tabName;
    switch (tabName) {
      case 'overview':
        $ionicScrollDelegate.scrollTop();
        break;
      case 'profile_setup':
        $ionicScrollDelegate.scrollTop();
        break;
      case 'email_option':
        $ionicScrollDelegate.scrollTop();
        break;
      case 'manage':
        $ionicScrollDelegate.scrollTop();
        break;
      case 'settings':
        $ionicScrollDelegate.scrollTop();
        break;
      case 'my_library':
        $ionicScrollDelegate.scrollTop();
        break;
      default:
        console.log("WARNING : unknown tab name")
    }
  };

  $scope.aboutGroups = [];
  $scope.personalGroups = [];
  $scope.hometownGroups = [];
  $scope.jobsGroups = [];
  $scope.educationGroups = [];
  $scope.interestGroups = [];
  $scope.emailAddressGroups = [];
  $scope.emailYouGroups = [];
  $scope.changePasswordGroups = [];
  for (var i = 0; i < 1; i++) {
    $scope.aboutGroups[i] = {
      name: i,
      items: []
    };
    for (var j = 0; j < 1; j++) {
      $scope.aboutGroups[i].items.push(i + '-' + j);
    }
  }

  for (var i = 0; i < 1; i++) {
    $scope.personalGroups[i] = {
      name: i,
      items: []
    };
    for (var j = 0; j < 1; j++) {
      $scope.personalGroups[i].items.push(i + '-' + j);
    }
  }

  for (var i = 0; i < 1; i++) {
    $scope.hometownGroups[i] = {
      name: i,
      items: []
    };
    for (var j = 0; j < 1; j++) {
      $scope.hometownGroups[i].items.push(i + '-' + j);
    }
  }

  for (var i = 0; i < 1; i++) {
    $scope.jobsGroups[i] = {
      name: i,
      items: []
    };
    for (var j = 0; j < 1; j++) {
      $scope.jobsGroups[i].items.push(i + '-' + j);
    }
  }

  for (var i = 0; i < 1; i++) {
    $scope.educationGroups[i] = {
      name: i,
      items: []
    };
    for (var j = 0; j < 1; j++) {
      $scope.educationGroups[i].items.push(i + '-' + j);
    }
  }

  for (var i = 0; i < 1; i++) {
    $scope.interestGroups[i] = {
      name: i,
      items: []
    };
    for (var j = 0; j < 1; j++) {
      $scope.interestGroups[i].items.push(i + '-' + j);
    }
  }

  for (var i = 0; i < 1; i++) {
    $scope.emailAddressGroups[i] = {
      name: i,
      items: []
    };
    for (var j = 0; j < 1; j++) {
      $scope.emailAddressGroups[i].items.push(i + '-' + j);
    }
  }

  for (var i = 0; i < 1; i++) {
    $scope.emailYouGroups[i] = {
      name: i,
      items: []
    };
    for (var j = 0; j < 1; j++) {
      $scope.emailYouGroups[i].items.push(i + '-' + j);
    }
  }

  for (var i = 0; i < 1; i++) {
    $scope.changePasswordGroups[i] = {
      name: i,
      items: []
    };
    for (var j = 0; j < 1; j++) {
      $scope.changePasswordGroups[i].items.push(i + '-' + j);
    }
  }

  /*
   * if given group is the selected group, deselect it
   * else, select the given group
   */
  $scope.toggleGroup = function(group) {
    if ($scope.isGroupShown(group)) {
      $scope.shownGroup = null;
    } else {
      $scope.shownGroup = group;
    }
  };
  $scope.isGroupShown = function(group) {
    return $scope.shownGroup === group;
  };



})

.controller('UploadCtrl', function($scope, $ionicSideMenuDelegate, $rootScope) {
  $ionicSideMenuDelegate.canDragContent(false);
  $rootScope.hideNavBar = false;
})

.controller('PrivacyCtrl', function($scope, $ionicSideMenuDelegate, $rootScope) {
  $ionicSideMenuDelegate.canDragContent(false);
  $rootScope.hideNavBar = false;
})

.controller('ContactCtrl', function($scope, $ionicSideMenuDelegate, $rootScope) {
  $ionicSideMenuDelegate.canDragContent(false);
  $rootScope.hideNavBar = false;
})

.controller('TermsCtrl', function($scope, $ionicSideMenuDelegate, $rootScope) {
  $ionicSideMenuDelegate.canDragContent(false);
  $rootScope.hideNavBar = false;
})

.controller('SearchResultCtrl', function($scope, $ionicSideMenuDelegate, $rootScope, AlbumService) {
  var searchtext = '';
  if ($rootScope.searchText)
    searchtext = $rootScope.searchText.toLowerCase();
  $ionicSideMenuDelegate.canDragContent(false);
  $rootScope.hideNavBar = false;
  var loadRecentVideo = function() {
    var query = {
      // UserName: $rootScope.User.UserName,
      isEnabled: 1,
      isReviewed: 1,
      LoadLibrary: false,
      LoadFavorites: false, // make it true for loading favorited videos
      LoadLiked: false, // make it true for loading liked videos
      Type: 0, // 0: for videos, 1: for audio
      PageNumber: 1,
      PageSize: 10,
      Term: searchtext, // search term if any
      Categories: '', // filter records by category
      Tags: '', // filter records by tags
      isFeatured: 2, // filter records by featured, 1: for featured, 0: for normal,
      Order: 'date_added desc',
      SearchType: 3
    }

    AlbumService.loadVideo(query, function(res) {

      $scope.recentVideos = res.data.data.Data;
    });

  }
  var loadRecentAudio = function() {
    var query = {
      // UserName: $rootScope.User.UserName,
      isEnabled: 1,
      isReviewed: 1,
      LoadLibrary: false,
      LoadFavorites: false, // make it true for loading favorited videos
      LoadLiked: false, // make it true for loading liked videos
      Type: 1, // 0: for videos, 1: for audio
      PageNumber: 1,
      PageSize: 10,
      Term: searchtext, // search term if any
      Categories: '', // filter records by category
      Tags: '', // filter records by tags
      isFeatured: 2, // filter records by featured, 1: for featured, 0: for normal,
      Order: 'date_added desc',
      SearchType: 3
    }

    AlbumService.loadVideo(query, function(res) {
      $scope.recentAudios = res.data.data.Data;
    });

  }
  var loadRecentPhoto = function() {
    var query = {
      isDisabled: 1,
      isApproved: 1,
      LoadLibrary: false,
      LoadFavorites: false, // make it true for loading favorited videos
      LoadLiked: false, // make it true for loading liked videos
      PageNumber: 1,
      PageSize: 10,
      Term: searchtext, // search term if any
      Categories: '',
      Tags: '',
      Order: "added_date desc",
      SearchType: 3
    }

    AlbumService.loadPhoto(query, function(res) {
      $scope.recentPhotos = res.data.data.Data;
    });

  }



  loadRecentVideo();
  loadRecentAudio();
  loadRecentPhoto();

})

.controller('VideoListCtrl', function($scope, $rootScope, $state) {


})

.controller('AudioListCtrl', function($scope, $rootScope, $state) {

})

.controller('PhotoListCtrl', function($scope, $rootScope, $state) {

})

;
