#!/usr/bin/env python
import socket
import struct

HOST = '127.0.0.1'
PORT = 5000
bytes = []
sample_clock = 0.0
last_data_sample = False
last_rds_sample = False
frame = 0

def do_data_bit(this_conversion):
    global frame
    frame = frame << 1
    if this_conversion:
        frame += 1

def process_stream_samples():
    global bytes
    while len(bytes) > 3:
        f = struct.unpack('f', ''.join(bytes[0:4]))[0]
        bytes = bytes[4:]
        # print f
        process_sample(f >= 0)

def process_sample(this_sample):
    global sample_clock
    global last_data_sample
    if this_sample ^ last_data_sample:
        if sample_clock > 10:
            sample_clock -= 0.1
        elif sample_clock < 10:
            sample_clock += 0.1
    last_data_sample = this_sample
    sample_clock += (100 / 14.73684210526316)
    if sample_clock >= 100:
        sample_clock -= 100
        # TODO now call dordsdatabit on this next line
        differential_decode(this_sample)

def differential_decode(this_sample):
    global last_rds_sample
    this_conversion = this_sample ^ last_rds_sample
    last_rds_sample = this_sample
    return this_conversion

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.bind((HOST, PORT))
s.listen(1)

conn,addr = s.accept()
print "Received connection from", addr
while True:
    data = conn.recv(4096)
    if not data:
        break
    for byte in data:
        bytes.append(byte)
    process_stream_samples();
