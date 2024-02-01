import socket
import threading
import time
import RPi.GPIO as GPIO

servoPIN = 17
GPIO.setmode(GPIO.BCM)
GPIO.setup(servoPIN, GPIO.OUT)
pwm = GPIO.PWM(servoPIN, 50)
pwm.start(0)
pwm.ChangeDutyCycle(9)

servo2PIN = 26
GPIO.setmode(GPIO.BCM)
GPIO.setup(servo2PIN, GPIO.OUT)
pwm2 = GPIO.PWM(servo2PIN, 50)
pwm2.start(0)
pwm2.ChangeDutyCycle(2)

TRIG = 23
ECHO = 24
GPIO.setup(TRIG, GPIO.OUT)
GPIO.setup(ECHO, GPIO.IN)


TRIG2 = 27
ECHO2 = 22
GPIO.setup(TRIG2, GPIO.OUT)
GPIO.setup(ECHO2, GPIO.IN)


TRIG3 = 5
ECHO3 = 6
GPIO.setup(TRIG3, GPIO.OUT)
GPIO.setup(ECHO3, GPIO.IN)

IN1 = 16
IN2 = 26
ENA_PIN = 12

GPIO.setup(IN1, GPIO.OUT)
GPIO.setup(IN2, GPIO.OUT)
GPIO.setup(ENA_PIN, GPIO.OUT)


GPIO.output(IN1, GPIO.HIGH)
GPIO.output(IN2, GPIO.LOW)

pwm1 = GPIO.PWM(ENA_PIN, 10)
pwm1.start(0)



def aciAyarla(aci):
    x = (1/180) * aci + 1
    duty = x * 5
    pwm.ChangeDutyCycle(duty)
    
def aci2Ayarla(aci):
    x = (1/180) * aci + 1
    duty = x * 5
    pwm2.ChangeDutyCycle(duty)
    
def doorSpeed(speed_level):
    try:
            if speed_level == "Slow":
                sleep_duration = 0.02
            elif speed_level == "Normal":
                sleep_duration = 0.01
            elif speed_level == "Fast":
                sleep_duration = 0.005
            else:
                print("Geçersiz hız seviyesi!")
                return
            for i in range(90, 270, 1):
                aciAyarla(i)
                time.sleep(sleep_duration)

    except KeyboardInterrupt:
        GPIO.cleanup()
        pwm.stop()
	
def closeDoor():
	for i in range(270,90,-1):
		aciAyarla(i)
		time.sleep(0.01)
	
def washTank(angle):
    try:
        if angle == "%30":
            for i in range(2, 62, 1):
                aci2Ayarla(i)
                time.sleep(0.02)
        elif angle == "%60":
            for i in range(2, 102, 1):
                aci2Ayarla(i)
                time.sleep(0.02)    
        elif angle == "%100":
            for i in range(2, 182, 1):
                aci2Ayarla(i)
                time.sleep(0.02)
        else:
            print("Geçersiz açı!")
            return
    
    except KeyboardInterrupt:
        GPIO.cleanup()
        pwm2.stop()

        
        
def distance():
    GPIO.output(TRIG, False)
    time.sleep(0.1)

    GPIO.output(TRIG, True)
    time.sleep(0.00001)
    GPIO.output(TRIG, False)

    pulse_start = time.time()
    while GPIO.input(ECHO) == 0:
        pulse_start = time.time()

    pulse_end = time.time()
    while GPIO.input(ECHO) == 1:
        pulse_end = time.time()

    pulse_duration = pulse_end - pulse_start
    distance = pulse_duration * 17150
    distance = round(distance, 2)
    return distance
    
    
def distance2():
    GPIO.output(TRIG2, False)
    time.sleep(0.1)

    GPIO.output(TRIG2, True)
    time.sleep(0.00001)
    GPIO.output(TRIG2, False)

    pulse_start2 = time.time()
    while GPIO.input(ECHO2) == 0:
        pulse_start2 = time.time()

    pulse_end2 = time.time()
    while GPIO.input(ECHO2) == 1:
        pulse_end2 = time.time()

    pulse_duration2 = pulse_end2 - pulse_start2
    distance2 = pulse_duration2 * 17150
    distance2 = round(distance2, 2)
    return distance2
    
    
def distance3():
    GPIO.output(TRIG3, False)
    time.sleep(0.1)

    GPIO.output(TRIG3, True)
    time.sleep(0.00001)
    GPIO.output(TRIG3, False)

    pulse_start3 = time.time()
    while GPIO.input(ECHO3) == 0:
        pulse_start3 = time.time()

    pulse_end3 = time.time()
    while GPIO.input(ECHO3) == 1:
        pulse_end3 = time.time()

    pulse_duration3 = pulse_end3 - pulse_start3
    distance3 = pulse_duration3 * 17150
    distance3 = round(distance3, 2)
    return distance3
    

def send_udp_message(message, server_ip="192.168.1.2", server_port=5555):
    client_socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    client_socket.sendto(message.encode("utf-8"), (server_ip, server_port))
    client_socket.close()

def tcp_receive_thread():
    server2_ip = "192.168.0.59"
    server2_port = 5555

    server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server_socket.bind((server2_ip, server2_port))
    server_socket.listen(1) 

    print(f"TCP Sunucu dinleniyor: {server2_ip}:{server2_port}")

    try:
        while True:
            conn, addr = server_socket.accept()
            print(f"Bağlantı alındı from {addr[0]}:{addr[1]}")

            data = conn.recv(1024)
            message = data.decode("utf-8")
            print(f"Gelen mesaj: {message}")

            if message == "Slow":
                doorSpeed(message)
            elif message == "Normal":
                doorSpeed(message)
            elif message == "Fast":
                doorSpeed(message)
            elif message == "CloseDoor":
                closeDoor()
            elif message == "Low":
                pwm1.ChangeDutyCycle(30)
            elif message == "Medium":
                pwm1.ChangeDutyCycle(65)
            elif message == "High":
                pwm1.ChangeDutyCycle(100)
            elif message == "CloseCooling":
                pwm1.ChangeDutyCycle(0)
            elif message == "%30":
                washTank(message)
            elif message == "%60":
                washTank(message)
            elif message == "%100":
                washTank(message)
            elif message == "DefaultAngle":
                pwm2.ChangeDutyCycle(2)

            conn.close()
    except KeyboardInterrupt:
        server_socket.close()




def main_thread():
    try:
        while True:
            dist = distance()
            dist2 = distance2()
            dist3 = distance3()
            
            if 5 <= dist < 10:
                message_to_send = "YesCar"
                print("YesCar")
                send_udp_message(message_to_send)
            elif dist < 5:
                message_to_send = "PassedCar"
                print("Passed")
                send_udp_message(message_to_send)
            else:
                message_to_send = "NoCar"
                send_udp_message(message_to_send)
            
            if dist2 < 3:
                message_to_send = "YesWash"
                print("YesWash")
                send_udp_message(message_to_send)
                
            if dist3 < 3:
                message_to_send = "YesCooling"
                print("YesCooling")
                send_udp_message(message_to_send)    
                
    except KeyboardInterrupt:
        GPIO.cleanup()
        pwm.stop()
        pwm1.stop()

        
        
        
if __name__ == "__main__":
    tcp_receive_thread = threading.Thread(target=tcp_receive_thread)
    main_thread = threading.Thread(target=main_thread)

    tcp_receive_thread.start()
    main_thread.start()

    try:
        while True:
            time.sleep(1)
    except KeyboardInterrupt:
        GPIO.cleanup()
        pwm.stop()
        udp_receive_thread.join()  
        main_thread.join()  