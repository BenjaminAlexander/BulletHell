import subprocess
import threading

def addPrefixToLine(line, prefix):
    return "[" + prefix + "]: " + line;

def flushToStdWithPrefix(data:str, prefix:str):
    if(data is not None):
        lines = data.splitlines();
        for line in lines:
            print(addPrefixToLine(line, prefix));

def flushToStd(popenObj, prefix):

    def flushToStdThread(stream, prefix):
        while popenObj.returncode is None:
            flushToStdWithPrefix(stream.readline(), prefix);
        flushToStdWithPrefix(stream.readline(), prefix);

    flushThread = threading.Thread(target=flushToStdThread, args=(popenObj.stdout, prefix));
    flushThread.start();

    flushThread = threading.Thread(target=flushToStdThread, args=(popenObj.stderr, prefix + " ERROR"));
    flushThread.start();

def popenWithStdFlushing(args, prefix):
    p = subprocess.Popen(args,
                        text=True,
                      stdout=subprocess.PIPE,
                      stdin=subprocess.PIPE,
                      stderr=subprocess.PIPE);
    flushToStd(p, prefix);
    return p;
