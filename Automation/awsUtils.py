import sys
import boto3
import pprint
from datetime import datetime
from dateutil.tz import tzutc
from botocore.exceptions import ClientError

#returns a list of dictionaries describing ec2 instances
def getInstances(ec2client):
    response = ec2client.describe_instances()
    
    instances = []
    for reservation in response['Reservations']:
        for instance in reservation['Instances']:
            instances.append(instance)
    return instances

def getStopTime(instance):
    stopTime = None
    for tag in instance['Tags']:
        if tag['Key'] == 'StopAt':
            stopTime = datetime.fromisoformat(tag['Value'])
    return stopTime

def stopInstance(ec2client, instance):
    response = ec2client.stop_instances(InstanceIds=[instance['InstanceId']], DryRun=False)
    print(response)
    
instance_id = 'i-058f49164458489ae'

ec2 = boto3.client('ec2')
instances = getInstances(ec2)

now = datetime.now(tz=tzutc())
for instance in instances:
    stopTime = getStopTime(instance)
    if stopTime != None and stopTime < now:
        stopInstance(ec2, instance)

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
