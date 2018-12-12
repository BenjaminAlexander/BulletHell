import awsUtils
import boto3

ec2 = boto3.resource('ec2')

#C:\Program Files (x86)\Go Agent\config\autoregister.properties
#testInstance = ec2.Instance('i-058f49164458489ae')
#awsUtils.startInstanceWithStopTime(testInstance, 10)

instance = awsUtils.createMetaserverEC2Instance(ec2, 720, "4f6482d7-9a7c-4ced-9b4d-694ee9f345c2")
