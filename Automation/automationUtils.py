import subprocess
import threading

def addPrefixToLine(line, prefix):
    return "[" + prefix + "]: " + line;

def flushBytesToStdWithPrefix(data:bytes, prefix:str):
    if(data is not None):
        strData = data.decode('utf-8');
        lines = strData.splitlines();
        for line in lines:
            print(addPrefixToLine(line, prefix));

def flushToStdWithPrefix(popenObj:subprocess.Popen, prefix:str):
    stdoutdata, stderrdata = popenObj.communicate();
    flushBytesToStdWithPrefix(stdoutdata, prefix);
    flushBytesToStdWithPrefix(stderrdata, prefix + " ERROR");
    

def logToStd(popenObj, prefix):

    def flushOutToStd():
        while popenObj.returncode is None:
            flushToStdWithPrefix(popenObj, prefix);
        flushToStdWithPrefix(popenObj, prefix);

    flushThread = threading.Thread(target=flushOutToStd);
    flushThread.start();

def popenWithStdFlushing(args, prefix):
    p = subprocess.Popen(args,
                      stdout=subprocess.PIPE,
                      stdin=subprocess.PIPE,
                      stderr=subprocess.PIPE);
    logToStd(p, prefix);
    return p;
