"""
Receive data from Pupil using ZMQ.
"""
import zmq
from msgpack import loads
import serial
from pupil_remote_control import Requester
import time
import socket

context = zmq.Context()
# open a req port to talk to pupil
#addr = '127.0.0.1'  # localhost
addr = '131.113.136.249'  # remote ip
req_port = "49694"  # same as in the pupil remote gui
req = context.socket(zmq.REQ)
req.connect("tcp://{}:{}".format(addr, req_port))
# ask for the sub port
req.send_string('SUB_PORT')
sub_port = req.recv_string()

# open a sub port to listen to pupil
sub = context.socket(zmq.SUB)
sub.connect("tcp://{}:{}".format(addr, sub_port))

dstip = "131.113.137.92"
dstport = 8888


# set subscriptions to topics
# recv just pupil/gaze/notifications
sub.setsockopt_string(zmq.SUBSCRIBE, unicode("pupil", encoding="latin-1"))
# sub.setsockopt_string(zmq.SUBSCRIBE, 'gaze')
# sub.setsockopt_string(zmq.SUBSCRIBE, 'notify.')
# sub.setsockopt_string(zmq.SUBSCRIBE, 'logging.')
# or everything:
# sub.setsockopt_string(zmq.SUBSCRIBE, '')

sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

count = 0

while True:
	try:
		topic = sub.recv_string()
		msg = sub.recv()
		msg = loads(msg, encoding='utf-8')
		#print("\n{}: {}".format(topic, msg))
		pupil_dilation = msg['diameter']
		if(pupil_dilation > 0):
			print (topic + "," + str(pupil_dilation))
			message = "5," + str(pupil_dilation)
			sock.sendto(message, (dstip, dstport))
			print ("UDP sent")
	except KeyboardInterrupt:
		break