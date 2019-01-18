from flask import Flask, url_for
from flask import request, make_response, Response, jsonify
from datetime import datetime
from fractal import run_stub
from random import random
import requests
import hashlib
import os
app = Flask(__name__)

client = None

import boto3
client = boto3.client('s3')
# Call S3 to list current buckets
response = client.list_buckets()
# Get a list of all bucket names from the response
buckets = [bucket['Name'] for bucket in response['Buckets']]
# Print out the bucket list
print("DEBUG: Bucket List: %s" % buckets)

class PrettyFloat(float):
    def __repr__(self):
        return '%.15g' % self

def pretty_float(obj):
    if isinstance(obj, float):
        return PrettyFloat(obj)
    elif isinstance(obj, str):
        try:
            return PrettyFloat(float(obj))
        except ValueError:
            return None
    return None
            
    

@app.route('/')
def homepage():
    the_time = datetime.now().strftime("%A, %d %b %Y %l:%M %p")
    print ("CHECKING AMAZZON:")
    print ("AWS_ACCESS_KEY_ID: ", os.environ.get("AWS_ACCESS_KEY_ID"))
    print ("AWS_SECRET_ACCESS_KEY: ", os.environ.get("AWS_SECRET_ACCESS_KEY"))

    return """
    <h1>Hello heroku</h1>
    <p>It is currently {time}.</p>

    <img src="http://loremflickr.com/600/400">
    """.format(time=the_time)

@app.route('/getcat')
def getcat():
    r = requests.get("http://loremflickr.com/600/400")

    # write to a file in the app's instance folder
    # come up with a better file name
    with app.open_instance_resource('downloaded_file', 'wb') as f:
        f.write(r.content)
        
    return 'Index Page'

@app.route('/getstub')
def getstub():
    a = random() if random() > 0.5 else -1*random()
    b = random() if random() > 0.5 else -1*random()

    the_time = datetime.now().strftime("%A, %d %b %Y %l:%M %p")
    
    cwd = os.getcwd()
    print(cwd)
    file_name = "img_"+str(a)+"_"+str(b)+".png"
    img_name = "static/"+file_name
    run_stub(a+b*1j, img_name)
    url = url_for('static', filename=file_name)

    import boto3
    client = boto3.client('s3')

    # Call S3 to list current buckets
    response = client.list_buckets()

    # Get a list of all bucket names from the response
    buckets = [bucket['Name'] for bucket in response['Buckets']]

    # Print out the bucket list
    print("Bucket List: %s" % buckets)
    client.upload_file(img_name, 'test-govno', file_name, ExtraArgs={'ACL':'public-read'})


    return """
    <h1>Hello heroku</h1>
    <p>It is currently {time}.</p>
    <p>Current cwd {cwd}. </p>
    <img src="{url}">
    """.format(time=the_time, cwd = cwd, url = url)

@app.route('/getfrac', methods = ['POST'])
def getfrac():
    if request.method == 'POST':
        try:
            body = request.json
            a = pretty_float(body.get("a", None))
            b = pretty_float(body.get("b", None))
            if a is not None and b is not None:                
                file_name = "img_"+str(a)+"_"+str(b)+".png"
                img_name = "static/"+file_name
                run_stub(a+b*1j, img_name)
                client.upload_file(img_name, 'test-govno', file_name, ExtraArgs={'ACL':'public-read'})
                resurl = "https://s3.amazonaws.com/test-govno/" + file_name
                os.remove(img_name)
                return jsonify(supurl = resurl)
            else:
                print("WARNING: Some data is empty. got body:", body, "a:", a, "b:", b)                
        except KeyError as err:
            print("WARNING: Bad post data, got body:", body, "err: ", err)
        except AttributeError as err:
            print("WARNING: Bad post data, got body:", body, "err: ", err)
    else:
        print("WARNING: Someone doing get request, ignoring")
    return Response(status=201)



if __name__ == '__main__':

    

    app.run(debug=True, use_reloader=True)
    
