var SSO = (function () {
	
	var redirectURL= location.href;//بعد از لاگین به این آدرس برگردد
	var showLoginFlag = true;
	
	var init = function (redirUrl) {//بعد از لودصفحه باید صدا زده شود تا فریم را در آخر صفحه بار گذاری کند
		if (window.addEventListener){
			addEventListener('message', msgListener, false);
		} else {
			attachEvent("onmessage", msgListener);
		}

        if(redirUrl && typeof redirUrl === 'string')
			redirectURL = redirUrl;
		
		if(Storage.getUserInfo() != null){//در سرور لوکال کاربر معتبر است
			return;
		}
		
		if (!document.getElementById('autFrame')) {
			var autFrame = document.createElement("iframe");
			autFrame.setAttribute("id", "autFrame");
			autFrame.setAttribute("src", AUTHENTICATION_SERVER + "/lib/api/sender.html");
			autFrame.setAttribute("style", "width:0px;height:0px;border:0px");
			document.getElementsByTagName('body')[0].appendChild(autFrame);
		}
		//درصورتی به هر دلیلی در اس اس  او مشکلی پیش آمد ری لود کند
		setTimeout(function(){
			if(showLoginFlag)
				showLoginPage(redirectURL); 
			}, 10000);
	};
	
	
	var msgListener = function (e){
		showLoginFlag = false;
		var ticket;
		try{			
			ticket = JSON.parse(e.data).ticket;
			if(ticket == "null"){
				showLoginPage(redirectURL);
				return;
			}
		}catch (err) {
			showLoginPage(redirectURL);
			return;
		}
		//if(e.origin !== 'https://davidwalsh.name') return;
		console.log('message received:  ' + e.origin + ' --> '+ e.data, e);
		//e.source.postMessage('holla back youngin! ', e.origin);
		
		//Call authentication server
		var handler = new Handler();
        handler.success = function (data) {
        	if(data.done){
        		setSSOUser(data.result);
        		console.log('after set sso user redirectUrl>' + redirectURL);
        		console.log('after set sso user location.href>' + location.href);
        		if(redirectURL != location.href)
        			location.href = redirectURL;
        		else{
        			location.reload();//onPageReady();
        		}
        		return;
        	}
        	else{
        		showLoginPage(redirectURL);
        		return;
        	}
        	
        };
        handler.error = function (jqXHR) {
        	console.log('Error in load user from aut server');
        	console.log(jqXHR);
        	showLoginPage();
        };

      	var data = {key:"ticket", value: ticket };
        Api.post({url: 'security/getUser', data: data, handler: handler});
        
	};
	

	var setSSOUser = function (u) {
	    // Action codes to check
	    u.action_insert = 1;
	    u.action_update = 2;
	    u.action_delete = 3;
	    u.action_search = 4;
	    u.action_view = 5;
	    u.action_print = 6;
	    u.action_export_excel = 7;
	    u.action_export_PDF = 8;
	    u.action_view_count_all = 9;
	    u.action_approve = 10;
	    u.action_reject = 11;
	    u.action_show_in_menu = 12;
        
	    u.hasAccess = function (usecase, action) {
	        var act = u.access[usecase];
	        return act != null && (act | (Math.pow(2, action))) == act;
	    }
	    user = u;
	    Storage.setUserInfo(u);
	};
	

	/*//باعث می شود با لود صفحه متد صدا زده شود
	if (window.addEventListener) {
		window.addEventListener('load', aut)
	} else {
		window.attachEvent('onload', aut)
	}*/
	
	 return {
		 init: init,
		 msgListener: msgListener,
		 setSSOUser: setSSOUser
	    }
})();
		
