
(function() {
 
	var client;
		client = {};
		client.init = function() {		
		client.socket = io.connect('http://127.0.0.1:4000');
		client.socket.on('client_kinect', function(data) {
		  return client.parseData(data.args);  
		});	
	};
	
	client.parseData = function(data){
		
		// console.log(data);
		
		var arr = data.split("/prop/");
		if(arr.length>1){
			
			var skeleton = new Object();
				skeleton.package = Number(arr[0]); // return indice de package : Si cet indice est identique aux quatres derniers packages, la kinect n'a plus de user .
				skeleton.id = Number(arr[1]); // retun skeleton id user . 
				skeleton.gesture = String(arr[2]); // return : null - left - right . 
				
			var i;
			var joints = arr[3].split("joint/");
			for(i=1; i<joints.length; i++){
				
				var prop = joints[i].split("/");
				skeleton[prop[0]] = new Object();
				skeleton[prop[0]].x = prop[1];
				skeleton[prop[0]].y = prop[2];
				skeleton[prop[0]].z = prop[3];			
			}
			
			/*
			/* ----------------------------------------
				skeleton.properties 
			/* ----------------------------------------
			
				- skeleton['hipcenter']			 
				- skeleton['spine']	
				- skeleton['head']	
				- skeleton['shoulderleft']	
				- skeleton['elbowleft']	
				- skeleton['wristleft']	
				- skeleton['handleft']	
				- skeleton['shouldercenter']	
				- skeleton['shoulderight']	
				- skeleton['elbowright']	
				- skeleton['wristright']	
				- skeleton['handright']	
				- skeleton['hipleft']	
				- skeleton['kneeleft']	
				- skeleton['ankleleft']	
				- skeleton['footleft']	
				- skeleton['hipright']	
				- skeleton['kneeright']	
				- skeleton['ankleright']	
				- skeleton['footright']	
			
			/* ----------------------------------------
				console.log( skeleton['hipcenter'].x );
			/* ----------------------------------------
			*/
			
			console.log(skeleton.package);	
			
		}
		
		
		
	}
	  
	$(function() {
		return client.init();
	});
  
}).call(this);


