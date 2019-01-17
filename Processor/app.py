from flask import Flask, url_for
from datetime import datetime
from fractal import run_stub
from random import random
import requests
import hashlib
import os
app = Flask(__name__)

@app.route('/')
def homepage():
    the_time = datetime.now().strftime("%A, %d %b %Y %l:%M %p")

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
    
    the_path = "test.png"
    cwd = os.getcwd()
    print(cwd)
    file_name = "img_"+str(a)+"_"+str(b)+".png"
    img_name = "static/"+file_name
    run_stub(a+b*1j, img_name)
    url = url_for('static', filename=file_name)
    return """
    <h1>Hello heroku</h1>
    <p>It is currently {time}.</p>
    <p>Current cwd {cwd}. </p>
    <img src="{url}">
    """.format(time=the_time, cwd = cwd, url = url)

if __name__ == '__main__':
    app.run(debug=True, use_reloader=True)
