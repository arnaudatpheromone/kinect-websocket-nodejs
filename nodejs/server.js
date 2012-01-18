
var data_kinect = 'null';
var io;
  	io = require('socket.io').listen(4000, '127.0.0.1');
  	io.sockets.on('connection', function(socket) {});
	
setInterval(function(){ onLoop() }, 33);
var onLoop = function(){ io.sockets.emit('client_kinect', { args: data_kinect } ); }
	
var net = require('net');
var server = net.createServer(function (socket) 
	{	
	socket.addListener("connect", function() { console.log("connect "); });
 	socket.addListener("data", function(data) { data_kinect = String(data); });
	});

	server.listen(8124, "127.0.0.1");
	// console.log("TCP server listening on port 8124 at 127.0.0.1");








