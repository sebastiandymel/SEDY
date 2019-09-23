from flask import Flask
app = Flask(__name__)

@app.route('/')
def hello():
    name = ""
    for i in range(1, 10000):
        name += "Dupa \n"
    return name

@app.route('/home')
def home():
    return "Home !1234"

@app.route('/name/<name>/<dupa>')
def hello_name(name, dupa):
    return "Hello {} {}!".format(name, dupa)

if __name__ == '__main__':
    app.run()