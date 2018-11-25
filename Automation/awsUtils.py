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
    for instance in instances:
        stopTime = getStopTime(instance)
        if stopTime != None and stopTime < now:
            print("Stoping instance: " + instance.id)
            response = instance.stop()




#newInstance = ec2.create_instances(LaunchTemplate={'LaunchTemplateName': 'BulletHellMetaServer'},
#                                   MaxCount=1,
#                                   MinCount=1,
#                                   Monitoring={'Enabled':True})


#testInstance = ec2.Instance('i-058f49164458489ae')
#startInstanceWithStopTime(testInstance, 5)


"""
print('start')
try:
    ec2.stop_instances(InstanceIds=[instance_id], DryRun=True)
except ClientError as e:
    if 'DryRunOperation' not in str(e):
        print('dry run exception')
        raise

# Dry run succeeded, run start_instances without dryrun
try:
    response = ec2.stop_instances(InstanceIds=[instance_id], DryRun=False)
    print(response)
except ClientError as e:
    print(e)
"""

