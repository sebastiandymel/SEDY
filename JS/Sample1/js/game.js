// ===============================
// GAME STATE VARIABLES
// ===============================
var ballRadius = 10;
var x = 0;
var y = 0;
var dx = 2;
var dy = -2;
var paddleHeight = 10;
var paddleWidth = 90;
var paddleX = 0;
var rightPressed = false;
var leftPressed = false;

var gameOverNotify = null;
var ctx = null;
var canvas = null;
var interval;

var brickRowCount = 6;
var brickColumnCount = 5;
var brickHeight = 20;
var brickPadding = 10;
var brickOffsetTop = 30;
var brickOffsetLeft = 10;
var bricks = [];
var color_brick = "#001ADD";

// ===============================
// STARTUP
// ===============================

window.onload = function() {
  canvas = document.getElementById("board");
  ctx = board.getContext("2d");

  x = canvas.width / 2;
  y = canvas.height - 30;
  paddleX = (canvas.width - paddleWidth) / 2;

  generateBricks();

  document.addEventListener("keydown", keyDownHandler, false);
  document.addEventListener("keyup", keyUpHandler, false);

  gameOverNotify = document.querySelector(".game-over-notify");
  gameOverNotify.addEventListener("click", function() {
    document.location.reload();
  });

  var interval = setInterval(draw, 10);
};

// ===============================
// USER INPUT HANDLERS
// ===============================

function keyDownHandler(e) {
  if (e.key == "Right" || e.key == "ArrowRight") {
    rightPressed = true;
  } else if (e.key == "Left" || e.key == "ArrowLeft") {
    leftPressed = true;
  }
}

function keyUpHandler(e) {
  if (e.key == "Right" || e.key == "ArrowRight") {
    rightPressed = false;
  } else if (e.key == "Left" || e.key == "ArrowLeft") {
    leftPressed = false;
  }
}

// ===============================
// DRAWING
// ===============================

function generateBricks() {
  for (var c = 0; c < brickColumnCount; c++) {
    bricks[c] = [];
    for (var r = 0; r < brickRowCount; r++) {
      var rndWidth = randomInRange(40, 80);
      bricks[c][r] = {
        x: 0,
        y: 0,
        status: 1,
        width: rndWidth
      };
    }
  }
}

function randomInRange(min, max) {
  return Math.floor(Math.random() * max + min);
}

function collisionDetection() {
  for (var c = 0; c < brickColumnCount; c++) {
    for (var r = 0; r < brickRowCount; r++) {
      var b = bricks[c][r];
      if (b.status == 1) {
        if (x > b.x && x < b.x + b.width && y > b.y && y < b.y + brickHeight) {
          dy = -dy;
          b.status = 0;
        }
      }
    }
  }
}

function drawBall() {
  ctx.beginPath();
  ctx.arc(x, y, ballRadius, 0, Math.PI * 2);
  ctx.fillStyle = "#0095DD";
  ctx.fill();
  ctx.closePath();
}

function drawPaddle() {
  ctx.beginPath();
  ctx.rect(paddleX, canvas.height - paddleHeight, paddleWidth, paddleHeight);
  ctx.fillStyle = "#0095DD";
  ctx.fill();
  ctx.closePath();
}

function drawBricks() {  
  for (var r = 0; r < brickRowCount; r++) {
    var offsetX = brickOffsetLeft;
    for (var c = 0; c < brickColumnCount; c++) {      
      if (bricks[c][r].status == 1) {
        // CALCULATE POSITION
        var brickX = brickPadding  + offsetX;
        var brickY = r * (brickHeight + brickPadding) + brickOffsetTop;
        bricks[c][r].x = brickX;
        bricks[c][r].y = brickY;        
        // DRAW BRICK RECT
        ctx.beginPath();
        ctx.rect(brickX, brickY, bricks[c][r].width, brickHeight);
        ctx.fillStyle = color_brick;
        ctx.fill();
        ctx.closePath();
      }      
      offsetX += bricks[c][r].width + brickOffsetLeft;
    }    
  }
}

function draw() {
  ctx.clearRect(0, 0, canvas.width, canvas.height);
  drawBricks();
  drawBall();
  drawPaddle();
  collisionDetection();

  if (x + dx > canvas.width - ballRadius || x + dx < ballRadius) {
    dx = -dx;
  }
  if (y + dy < ballRadius) {
    dy = -dy;
  } else if (y + dy > canvas.height - ballRadius) {
    if (x > paddleX && x < paddleX + paddleWidth) {
      dy = -dy;
    } else {
      gameOverNotify.style.display = "flex";
      clearInterval(interval);
      return;
    }
  }

  if (rightPressed && paddleX < canvas.width - paddleWidth) {
    paddleX += 7;
  } else if (leftPressed && paddleX > 0) {
    paddleX -= 7;
  }

  x += dx;
  y += dy;
}
