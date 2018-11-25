import awsUtils
import boto3

ec2 = boto3.resource('ec2')

testInstance = ec2.Instance('i-058f49164458489ae')
awsUtils.startInstanceWithStopTime(testInstance, 10)

