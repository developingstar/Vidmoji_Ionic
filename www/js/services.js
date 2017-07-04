angular.module('starter.services', [])
  .factory('MapLoadingScreen', function($ionicLoading) {
    return {
      loading: null,
      show: function() {
        $ionicLoading.show({
          content: '',
          /*
                            template: '<div class="row" style="color: #55B750;"><div><ion-spinner icon="android" style="color: #55B750!important;"></ion-spinner></div><div style="padding-left: 10px;font-weight: 500;vertical-align: middle;padding-top: 5px;font-size: 12px;">Authenticating...</div></div>',
          */
          template: '<div><img src="img/giphy.gif" width="200px" height="200px"></div>',
          animation: 'fade-in',
          noBackdrop: true,
          showBackdrop: false,
          maxWidth: 400,
          maxHeight: 100,
          showDelay: 200
        });
      },
      hide: function() {
        $ionicLoading.hide();
      }
    }
  })

.factory('LoadingScreen', function($ionicLoading) {
  return {
    loading: null,
    show: function() {
      $ionicLoading.show({
        content: '',
        /*
         template: '<div class="row" style="color: #55B750;"><div><ion-spinner icon="android" style="color: #55B750!important;"></ion-spinner></div><div style="padding-left: 10px;font-weight: 500;vertical-align: middle;padding-top: 5px;font-size: 12px;">Authenticating...</div></div>',
         */
        template: '<div class="loading-bacground"><ion-spinner icon="android"></ion-spinner></div>',
        animation: 'fade-in',
        noBackdrop: true,
        showBackdrop: false,
        maxWidth: 400,
        maxHeight: 100,
        showDelay: 200
      });
    },
    hide: function() {
      $ionicLoading.hide();
    }
  }
})

.filter('parseUrl', function() {
  var //URLs starting with http://, https://, or ftp://
    replacePattern1 = /(\b(https?|ftp):\/\/[-A-Z0-9+&@#\/%?=~_|!:,.;]*[-A-Z0-9+&@#\/%=~_|])/gim,
    //URLs starting with "www." (without // before it, or it'd re-link the ones done above).
    replacePattern2 = /(^|[^\/])(www\.[\S]+(\b|$))/gim,
    //Change email addresses to mailto:: links.
    replacePattern3 = /(^|[^\/])(www\.[\S]+(\b|$))/gim;

  return function(text, target, otherProp) {
    angular.forEach(text.match(replacePattern1), function(url) {
      text = text.replace(replacePattern1, "<a href=\"$1\" target=\"_blank\">$1</a>");
    });
    angular.forEach(text.match(replacePattern2), function(url) {
      text = text.replace(replacePattern2, "$1<a href=\"http://$2\" target=\"_blank\">$2</a>");
    });
    angular.forEach(text.match(replacePattern3), function(url) {
      text = text.replace(replacePattern3, "<a href=\"mailto:$1\">$1</a>");
    });

    return text;
  };
})

/* // Loading Screen
.factory('LoadingScreen', function ($ionicLoading, $rootScope) {
  return {
    loading: null,
    show: function (message) {
      $rootScope.LoadingScreenClosed=false;
      if(message==null){
        $rootScope.msg='Loading...';
      }
      else{
        $rootScope.msg=message;
      }

      $ionicLoading.show({
        content: '',
        template: '<div class="loadingBg"><i class="ion-plus" style="font-size: 40px;padding: 10px;"></i><div style="padding: 10px;">{{msg}}</div></div>',
        /!* templateUrl:'templates/dashboard.html',*!/
        animation: 'fade-in',
        showBackdrop: true,
        maxWidth: 400,
        maxHeight: 100,
        showDelay: 200
      });
      /!* $timeout(function () {
       if($rootScope.LoadingScreenClosed==false){
       $ionicLoading.hide();
       $rootScope.popupMessage('Its taking time more than usual');
       }
       }, 2000);*!/
    },
    hide: function () {
      $ionicLoading.hide();
      $rootScope.LoadingScreenClosed=true;
    }
  }
})*/


.factory('HTTP', function($http, HOST, $rootScope, $ionicLoading) {
  return {
    get: function(url) {

      return $http({
        method: 'GET',
        url: url
      });
    },
    post: function(url, params) {
      var promise = $http({
        method: 'POST',
        url: HOST + url,
        headers: {
          'Content-type': 'application/json'
        },
        data: params
      });
      return promise;
    },
    uploadUsingDevice: function(url, file, params, modal) {
      // alert(JSON.stringify(params));
      var options = new FileUploadOptions();
      options.fileKey = "picture";
      options.chunkedMode = false;
      options.httpMethod = 'POST';
      options.params = params;
      options.headers = {
        'Content-Type': undefined
      };
      var ft = new FileTransfer();
      ft.upload(file, encodeURI(HOST + url), function(success) {
        $ionicLoading.hide();
        var str = JSON.stringify(success.response);
        if (str.indexOf("Event uploaded successfully") != -1) {
          $rootScope.DisplayMessage('Event uploaded successfully');
        } else {
          $rootScope.DisplayMessage('Something went wrong');
        }
        //alert(success.response);
      }, function(err) {
        $ionicLoading.hide();
        $rootScope.popupMessage('Something went wrong');
        // alert(JSON.stringify(err));
      }, options);
    }
  }
})

.factory('settings', function() {
  return {
    noImageUrl: "img/dummy_profile_pic.png"
  };
})

.factory('settings_events', function() {
  return {
    noImageUrlEvent: "img/takepic.png"
  };
})

.directive('dir', function($compile, $parse) {
    return {
      restrict: 'E',
      link: function(scope, element, attr) {
        scope.$watch(attr.content, function() {
          element.html($parse(attr.content)(scope));
          $compile(element.contents())(scope);
        }, true);
      }
    }
  })
  .service('CommonService', [function($http) {
    $http.post('/someUrl', data, config).then(function(res) {

    }, function(error) {});
  }])

.service('AlbumService', function($http) {
  var service = {};
  service.loadVideo = function(query, callback) {
    $http.post("http://staging.vidmoji.com/api/videos/process.ashx?action=load_videos",
      query).then(function(res) {
      callback(res);
      console.log(res);
    }, function(error) {
      var data = {
        status: 'error',
        message: 'Can not connect server. Please try again',
        role: ''
      }
      callback(data);
      console.log(error);

    });
  }
  service.loadPhoto = function(query, callback) {
    $http.post("http://staging.vidmoji.com/api/photos/process.ashx?action=load_photos",
      query).then(function(res) {
      callback(res);
      console.log(res);
    }, function(error) {
      var data = {
        status: 'error',
        message: 'Can not connect server. Please try again',
        role: ''
      }
      callback(data);
      console.log(error);

    });
  }

  service.loadMediaCategory = function(query, callback) {
    $http.post("http://staging.vidmoji.com/api/categories/process.ashx?action=load_categories",
      query).then(function(res) {
      callback(res);
      console.log(res);
    }, function(error) {
      var data = {
        status: 'error',
        message: 'Can not connect server. Please try again',
        role: ''
      }
      callback(data);
      console.log(error);

    });
  }


  service.getdetailaudio = function(audioid, callback) {
    $http.post("http://staging.vidmoji.com/api/videos/process.ashx?action=fetch_record&vid=" + audioid, {}).then(function(res) {
      callback(res);
      console.log(res);
    }, function(error) {
      var data = {
        status: 'error',
        message: 'Can not connect server. Please try again',
        role: ''
      }
      callback(data);
      console.log(error);

    });
  }

  service.getdetailphoto = function(imageid, callback) {
    $http.post("http://staging.vidmoji.com/api/videos/process.ashx?action=fetch_record&vid=" + imageid, {}).then(function(res) {
      callback(res);
      console.log(res);
    }, function(error) {
      var data = {
        status: 'error',
        message: 'Can not connect server. Please try again',
        role: ''
      }
      callback(data);
      console.log(error);

    });
  }

  service.getdetailvideo = function(videoid, callback) {
    $http.post("http://staging.vidmoji.com/api/videos/process.ashx?action=fetch_record&vid=" + videoid, {}).then(function(res) {
      callback(res);
      console.log(res);
    }, function(error) {
      var data = {
        status: 'error',
        message: 'Can not connect server. Please try again',
        role: ''
      }
      callback(data);
      console.log(error);

    });
  }

  return service;
})

.service('PhotoService', function($http) {

})

.service('AccountService', function($http) {
  var service = {}
  service.loginfacebook = function(facebookdata, callback) {
    var stringdata = "uid=10000&uname=tommy775&nm=shane&fn=shane&ln=michael&gn=male&bt=12/4/1992&eml=testing@mediasoftpro.com&loc=usa";
    $http.post(
      ' http://staging.vidmoji.com/handlers/signup.ashx?' + stringdata,
      data).then(function(res) {

      callback(res);
      console.log(res);
    }, function(error) {
      var data = {
        status: 'error',
        message: 'Can not connect server. Please try again',
        role: ''
      }
      callback(data);
      console.log(error);

    });
  }

  service.login = function(logindata, callback) {
    $http.post(
      'http://staging.vidmoji.com/api/user/process.ashx?action=login',
      logindata).then(function(res) {

      callback(res);
      console.log(res);
    }, function(error) {
      var data = {
        status: 'error',
        message: 'Can not connect server. Please try again',
        role: ''
      }
      callback(data);
    });
  }

  service.signup = function(signupdata, callback) {
    $http.post(
      'http://staging.vidmoji.com/api/user/process.ashx?action=register',
      signupdata).then(function(res) {
      callback(res);
    }, function(error) {
      var data = {
        status: 'error',
        message: 'Can not connect server. Please try again',
        role: ''
      }
      callback(data);
    });


  }

  service.updateprofile = function(user, callback) {
    var objUser = {
      UserName: user.UserName,
      AboutMe: user.AboutMe,
      Website: user.Website,
      FirstName: user.FirstName,
      LastName: user.LastName,
      Gender: user.Gender,
      HometTown: user.HometTown,
      CurrentCity: user.CurrentCity,
      Zipcode: user.Zipcode,
      CountryName: user.CountryName,
      Occupations: user.Occupations,
      Companies: user.Companies,
      Schools: user.Schools,
      Interests: user.Interests
    };
    $http.post(
      'http://staging.vidmoji.com/api/user/process.ashx?action=update_profile',
      objUser).then(function(res) {
      callback(res);
    }, function(error) {
      var data = {
        status: 'error',
        message: 'Can not connect server. Please try again',
        role: ''
      }
      callback(data);
    });
  }

  service.updateemail = function(user, callback) {
    var objUser = {
      UserName: user.UserName,
      Password: user.currentpass,
      Email: user.Email
    };
    $http.post(
      'http://staging.vidmoji.com/api/user/process.ashx?action=email_options',
      objUser).then(function(res) {
      callback(res);
    }, function(error) {
      var data = {
        status: 'error',
        message: 'Can not connect server. Please try again',
        role: ''
      }
      callback(data);
    });
  }

  service.updatepassword = function(user, callback) {
    var objUser = {
      UserName: user.UserName
    };

    var url = "http://staging.vidmoji.com/api/user/process.ashx?action=change_password&op=" + user.oldpassword + "&np" + user.newpassword + "";

    $http.post(url, objUser).then(function(res) {
      callback(res);
    }, function(error) {
      var data = {
        status: 'error',
        message: 'Can not connect server. Please try again',
        role: ''
      }
      callback(data);
    });
  }



  return service;
});
