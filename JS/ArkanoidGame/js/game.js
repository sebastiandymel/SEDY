// ===============================
// GAME STATE VARIABLES
// ===============================
const debug = true;
var ballRadius = 15;
var x = 0;
var y = 0;
var dx = 2;
var dy = -2;
var platformHeight = 20;
var platformWidth = 90;
var platformX = 0;
var rightPressed = false;
var leftPressed = false;
var refreshRate_ms = 20;
var gameOverNotify = null;
var gameWonNotify = null;
var scoreNotify = null;
var highestScoreNotify = null;
var ctx = null;
var canvas = null;
var interval;
var waitsForUserInput = false;

const color_brick = "#001ADD";
const color_platform = "#0095DD";
const color_ball = "#0095DD";

var bricksDefinition = {
  rowCount: 4,
  columnCount: 6,
  height: 20,
  padding: 10,
  offsetTop: 30,
  offsetLeft: 10,
  bricks: []
};

var gameLevel = -1;
const maxLevel = 4;
var score = 0;
var highestScore = 0;

// Level                         0    1    2    3   4
var platformWidtPerGameLevel = [400, 200, 100, 90, 70];
var platformSpeedPerGameLevel = [11, 12, 15, 20, 30];
var knownGameElements = {
  Canvas: "board",
  GameOver: ".game-over-notify",
  GameWon: ".game-won-notify",
  GameLevelTag: "_level",
  Score: "_score",
  HighestScore: "_highscore",
  DebugBtn: "_debugBtn"
};

// ===============================
// STARTUP
// ===============================

window.onload = function() {
  initializeGameElements();
  nextLevel();
  hookEvents();
};

function initializeGameElements() {
  canvas = document.getElementById(knownGameElements.Canvas);
  scoreNotify = document.getElementById(knownGameElements.Score);
  highestScoreNotify = document.getElementById(knownGameElements.HighestScore);
  ctx = board.getContext("2d");
  gameOverNotify = document.querySelector(knownGameElements.GameOver);
  gameWonNotify = document.querySelector(knownGameElements.GameWon);
}

function setDefaultBallPosition() {
  x = canvas.width / 2;
  y = canvas.height - 30;
}

function hookEvents() {
  document.addEventListener("keydown", keyDownHandler, false);
  document.addEventListener("keyup", keyUpHandler, false);

  // GAME OVER
  gameOverNotify.addEventListener("click", function() {
    restartGame();
  });

  // NEXT LEVEL
  gameWonNotify.addEventListener("click", function() {
    nextLevel();
    clearOverlays();
    waitsForUserInput = false;
  });

  // DEBUG
  if (debug) {
    var debugBtn = document.getElementById(knownGameElements.DebugBtn);
    debugBtn.style.display = "flex";
    debugBtn.addEventListener("click", function() {
      for (var r = 0; r < bricksDefinition.rowCount; r++) {
        for (var c = 0; c < bricksDefinition.columnCount; c++) {
          bricksDefinition.bricks[c][r].status = 0;
        }
      }
    });
  }
}

function restartGame() {
  gameLevel = -1;
  score = 0;
  nextLevel();
  clearOverlays();
  waitsForUserInput = false;
}

function clearOverlays() {
  gameWonNotify.style.display = "none";
  gameOverNotify.style.display = "none";
}

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
  for (var c = 0; c < bricksDefinition.columnCount; c++) {
    bricksDefinition.bricks[c] = [];
    for (var r = 0; r < bricksDefinition.rowCount; r++) {
      //var rndWidth = randomInRange(80, 80);
      bricksDefinition.bricks[c][r] = {
        x: 0,
        y: 0,
        status: 1,
        width: 80
      };
    }
  }
}

function randomInRange(min, max) {
  return Math.floor(Math.random() * (max - min) + min);
}

function collisionDetection() {
  for (var c = 0; c < bricksDefinition.columnCount; c++) {
    for (var r = 0; r < bricksDefinition.rowCount; r++) {
      var b = bricksDefinition.bricks[c][r];
      if (b.status == 1) {
        if (
          x > b.x &&
          x < b.x + b.width &&
          y > b.y &&
          y < b.y + bricksDefinition.height
        ) {
          dy = -dy;
          b.status = 0;
          score += 10;
        }
      }
    }
  }
}

function drawBall() {
  ctx.beginPath();
  ctx.arc(x, y, ballRadius, 0, Math.PI * 2);
  ctx.fillStyle = color_ball;
  ctx.fill();
  ctx.closePath();
}

function drawPlatform() {
  ctx.beginPath();
  ctx.rect(
    platformX,
    canvas.height - platformHeight,
    platformWidth,
    platformHeight
  );
  ctx.fillStyle = color_platform;
  ctx.fill();
  ctx.closePath();
}

function drawBricks() {
  bricksDefinition.offsetLeft =
    (canvas.width -
    bricksDefinition.columnCount * 80 -
    (bricksDefinition.columnCount - 1) * bricksDefinition.padding) / 2;

  for (var r = 0; r < bricksDefinition.rowCount; r++) {
    var offsetX = bricksDefinition.offsetLeft;
    for (var c = 0; c < bricksDefinition.columnCount; c++) {
      if (bricksDefinition.bricks[c][r].status == 1) {
        // CALCULATE POSITION
        var brickX = offsetX;
        var brickY =
          r * (bricksDefinition.height + bricksDefinition.padding) +
          bricksDefinition.offsetTop;
        bricksDefinition.bricks[c][r].x = brickX;
        bricksDefinition.bricks[c][r].y = brickY;
        // DRAW BRICK RECT
        ctx.beginPath();
        ctx.rect(
          brickX,
          brickY,
          bricksDefinition.bricks[c][r].width,
          bricksDefinition.height
        );
        ctx.fillStyle = color_brick;
        ctx.fill();
        ctx.closePath();
      }
      offsetX +=
        bricksDefinition.bricks[c][r].width + bricksDefinition.padding;
    }
  }
}

function drawScore() {
  scoreNotify.innerText = score;
  if (score > highestScore) {
    highestScore = score;
    highestScoreNotify.innerText = highestScore;
  }
}

function draw() {
  if (waitsForUserInput || gameLevel < 0) {
    return;
  }

  if (gameLevel == maxLevel)
  {    
    alert("You won the game! Congratulations!\nPress OK to start again.");
    clearInterval(interval);
    restartGame();
    return;
  }

  ctx.clearRect(0, 0, canvas.width, canvas.height);

  if (hasAnyBrick() == false) {
    gameWonNotify.style.display = "flex";
    clearInterval(interval);
    waitsForUserInput = true;
    return;
  }

  drawBricks();
  drawBall();
  drawPlatform();
  collisionDetection();
  drawScore();

  if (x + dx > canvas.width - ballRadius || x + dx < ballRadius) {
    dx = -dx;
  }
  if (y + dy < ballRadius) {
    dy = -dy;
  } else if (y + dy > canvas.height - ballRadius) {
    if (x > platformX && x < platformX + platformWidth) {
      dy = -dy;
    } else {
      gameOverNotify.style.display = "flex";
      waitsForUserInput = true;
      clearInterval(interval);
      return;
    }
  }

  var platformSpeed = platformSpeedPerGameLevel[gameLevel];
  if (rightPressed && platformX < canvas.width - platformWidth) {
    platformX += platformSpeed;
  } else if (leftPressed && platformX > 0) {
    platformX -= platformSpeed;
  }

  x += dx;
  y += dy;
}

function updatePlatform() {
  platformWidth = platformWidtPerGameLevel[gameLevel];
}

function nextLevel() {
  platformX = (canvas.width - platformWidth) / 2;
  this.gameLevel = this.gameLevel + 1;
  var levelTag = document.getElementById(knownGameElements.GameLevelTag);
  levelTag.innerText = this.gameLevel + 1; // zero-based
  setDefaultBallPosition();
  updatePlatform();
  generateBricks();

  interval = setInterval(draw, refreshRate_ms);

  var speedFactor = 3 + gameLevel / 2.0;
  dx = (speedFactor * refreshRate_ms) / 10;
  dy = (-speedFactor * refreshRate_ms) / 10;
}

function hasAnyBrick() {
  for (var r = 0; r < bricksDefinition.rowCount; r++) {
    for (var c = 0; c < bricksDefinition.columnCount; c++) {
      if (bricksDefinition.bricks[c][r].status > 0) {
        return true;
      }
    }
  }
  return false;
}
