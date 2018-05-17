from http.server import BaseHTTPRequestHandler, HTTPServer
from urllib.parse import urlparse

# HTTPRequestHandler class
class testHTTPServer_RequestHandler(BaseHTTPRequestHandler):
 
  # GET
  def do_GET(self):
        # Send response status code
        self.send_response(200)

        # Send headers
        self.send_header('Content-type','text/html')
        self.end_headers()

        #
        # PARSE HEADER
        #
        
        query = urlparse(self.path).query
        query_components = dict(qc.split("=") for qc in query.split("&"))
 
        # Send message back to client
        message = "Hello world!" + query_components["imie"]

        for i in range(10000):        
            message = message + str(i) + "\n"
             
        # Write content as utf-8 data
        self.wfile.write(bytes(message, "utf8"))
        return
 
def run():
  print('starting server...')
 
  # Server settings
  # Choose port 8080, for port 80, which is normally used for a http server, you need root access
  server_address = ('127.0.0.1', 8081)
  httpd = HTTPServer(server_address, testHTTPServer_RequestHandler)
  print('running server...')
  httpd.serve_forever()
 
 
run()
