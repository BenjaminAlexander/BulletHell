import sys
import boto3
import pprint
from datetime import datetime
from datetime import timedelta
from dateutil.tz import tzutc
from botocore.exceptions import ClientError

pp = pprint.PrettyPrinter()

def getStopTime(instance):
    stopTime = None
    if instance is not None:
        for tag in instance.tags:
            if tag['Key'] == 'StopAt':
                stopTime = datetime.fromisoformat(tag['Value'])
    return stopTime

def startInstanceWithStopTime(instance, deltaMinutes):
    stopTime = datetime.now(tz=tzutc()) + timedelta(minutes=deltaMinutes)
    instance.create_tags(Tags=[{'Key':'StopAt', 'Value':stopTime.isoformat()}])
    instance.start()

def stopExpiredInstances(instances):
    now = datetime.now(tz=tzutc())
    if instances is not None:
        for instance in instances:
            stopTime = getStopTime(instance)
            if stopTime != None and stopTime < now:
                print("Stoping instance: " + instance.id)
                response = instance.stop()

def createMetaserverEC2Instance(ec2, deltaMinutes, autoregisterkey):
    stopTime = datetime.now(tz=tzutc()) + timedelta(minutes=deltaMinutes)
    newInstances = ec2.create_instances(LaunchTemplate={'LaunchTemplateName': 'BulletHellMetaServer'},
                                        KeyName='MyFirstKey.pem',
                                        SecurityGroupIds=[
                                            'sg-00a22b0befc186b4c',
                                        ],
                                        UserData=
                                        """<powershell>
mkdir "C:\\Program Files (x86)\\Go Agent\\config"
$key = "4f6482d7-9a7c-4ced-9b4d-694ee9f345c2"
$resources = "Windows,EC2"
$UserInfoToFile = @"
agent.auto.register.key=$key
agent.auto.register.resources=$resources
"@
$UserInfoToFile | Out-File -FilePath "C:\\Program Files (x86)\\Go Agent\\config\\autoregister.properties" -Encoding ASCII
Invoke-WebRequest -OutFile C:\\Users\\Administrator\\Downloads\\go-agent-18.11.0-8024-jre-64bit-setup.exe https://download.gocd.org/binaries/18.11.0-8024/win/go-agent-18.11.0-8024-jre-64bit-setup.exe
C:\\Users\\Administrator\\Downloads\\go-agent-18.11.0-8024-jre-64bit-setup.exe /S /START_AGENT=YES /SERVERURL=`"https://gocd.benalexander.org:8154/go`"
</powershell>""",
                                        InstanceType='t2.micro',
                                        MaxCount=1,
                                        MinCount=1,
                                        Monitoring={'Enabled':True},
                                        TagSpecifications=[{
                                            'ResourceType':'instance',
                                            'Tags': [{
                                                'Key': 'StopAt',
                                                'Value': stopTime.isoformat()
                                            }]
                                        }])
    return newInstances[0]
