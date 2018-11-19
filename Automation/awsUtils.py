import sys
import boto3
import pprint
from datetime import datetime
from dateutil.tz import tzutc
from botocore.exceptions import ClientError

def getStopTime(instance):
    stopTime = None
    for tag in instance.tags:
        if tag['Key'] == 'StopAt':
            stopTime = datetime.fromisoformat(tag['Value'])
    return stopTime
    
ec2 = boto3.resource('ec2')
instances = ec2.instances.all()

now = datetime.now(tz=tzutc())
for instance in instances:
    stopTime = getStopTime(instance)
    if stopTime != None and stopTime < now:
        response = instance.stop()
        print(response)

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
print('finish')
