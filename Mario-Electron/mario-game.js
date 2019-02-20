// =========================================================================
// GAME BOARD DEFINITION
// . - air
// P - player
// # - wall, player cannot walk through it
// ? - unknown bonus
// O - coin
// % - Level exit
// =========================================================================
var levelData = null;

// =========================================================================
// Global game settings
// =========================================================================
const refresh_ms = 30;
const tileSize = 16;
const tilesHorizontal = 48;
const playerVerticalOffset = 6;
const playerMoveSize = tileSize / 3;
const playerJumpSize = tileSize * 2.5;
const coinScore = 5;
// =========================================================================
// Fields
// =========================================================================
var ctx = null;
var canvas = null;
var gameObjects = {
  player: null,
  wall: null,
  bonus: null,
  coin: null,
  princes: null
};
var score = 0;

var keyboard = {
  left: false,
  right: false,
  up: false
};
var player = {
  x: 0,
  y: 0
};
var isJumping = false;
var jumpDistance = 0;
// Array of blocking elements. Each element has
// {x, y} - starting cooridantes
// width, height - size of this blocking element
var blockers = [];

// =========================================================================
// GAME INITIALIZATION
// =========================================================================
window.onload = function() {
  document.addEventListener("keydown", keyDownHandler, false);
  document.addEventListener("keyup", keyUpHandler, false);
  loadLevelData(1, initializeCanvas);
  initializeGameObjects();
  setInterval(draw, refresh_ms);
};

function loadLevelData(levelNumber, callback) {
  console.log("Loading level nuber " + levelNumber);
  var prefix = "levels/level";
  if (levelNumber < 10) {
    prefix += "0";
  }
  var file = prefix + levelNumber + ".dat";
  var fs = require("fs");
  fs.readFile(file, "utf-8", (err, data) => {
    if (err) {
      alert("An error ocurred reading the file :" + err.message);
      return;
    }
    console.log("The level content is : \n" + data);
    var levelData = Array.from(data.replace(/(\r\n|\n|\r)/gm, ""));
    callback(levelData);
  });
}

function initializeCanvas(data) {
  levelData = data;
  canvas = document.getElementById("game-board");
  ctx = canvas.getContext("2d");
  canvas.setAttribute("width", tilesHorizontal * tileSize);
  canvas.setAttribute("height", (levelData.length / 48) * tileSize);
}

function initializeGameObjects() {
  var imgs_paths = {
    player: "imgs/mario_1.png",
    wall: "imgs/wall.png",
    bonus: "imgs/bonus.png",
    coin: "imgs/coin.png",
    princes: "imgs/princes.png"
  };

  gameObjects.player = new Image();
  gameObjects.player.src = imgs_paths.player;

  gameObjects.wall = new Image();
  gameObjects.wall.src = imgs_paths.wall;

  gameObjects.bonus = new Image();
  gameObjects.bonus.src = imgs_paths.bonus;

  gameObjects.coin = new Image();
  gameObjects.coin.src = imgs_paths.coin;

  gameObjects.princes = new Image();
  gameObjects.princes.src = imgs_paths.princes;
}

// =========================================================================
// KEYBOARD NAVIGATION
// =========================================================================

function keyDownHandler(e) {
  if (e.key == "Left" || e.key == "ArrowLeft" || e.key == "A" || e.key == "a") {
    keyboard.left = true;
  } else if (
    e.key == "Right" ||
    e.key == "ArrowRight" ||
    e.key == "D" ||
    e.key == "d"
  ) {
    keyboard.right = true;
  } else if (
    e.key == "Up" ||
    e.key == "ArrowUp" ||
    e.key == "w" ||
    e.key == "W"
  ) {
    keyboard.up = true;
  }
}

function keyUpHandler(e) {
  if (e.key == "Left" || e.key == "ArrowLeft" || e.key == "A" || e.key == "a") {
    keyboard.left = false;
  } else if (
    e.key == "Right" ||
    e.key == "ArrowRight" ||
    e.key == "D" ||
    e.key == "d"
  ) {
    keyboard.right = false;
  } else if (
    e.key == "Up" ||
    e.key == "ArrowUp" ||
    e.key == "w" ||
    e.key == "W"
  ) {
    keyboard.up = false;
  }
}

// =========================================================================
// DRAWING - GAME LOOP
// =========================================================================

function draw() {
  ctx.clearRect(0, 0, canvas.width, canvas.height);
  blockers = [];
  if (levelData != null) {
    drawLevel(levelData);
  }
}

function drawLevel(levelToDraw) {
  var rows = levelToDraw.length / tilesHorizontal;
  var columns = tilesHorizontal;

  var startx = 0;
  var starty = 0;

  var playerPosition_x = null;
  var playerPosition_y = null;

  var walls = [];
  var wallsIndex = 0;
  var bonus = [];
  var bonusIndex = 0;
  var coins = [];
  var coinsIndex = 0;

  var princesPosition_x = null;
  var princesPosition_y = null;

  for (var j = 0; j < rows; j++) {
    for (i = 0; i < columns; i++) {
      var index = j * tilesHorizontal + i;
      const element = levelToDraw[index];
      if (element == "#") {
        walls[wallsIndex++] = { x: startx, y: starty };
      } else if (element == "P") {
        playerPosition_y = starty + player.y - playerVerticalOffset;
        playerPosition_x = startx + player.x;
      } else if (element == "O") {
        if (
          collide(
            toTileRect(playerPosition_x, playerPosition_y),
            toTileRect(startx, starty)
          )
        ) {
          levelToDraw[index] = ".";
          score += coinScore;
        } else {
          coins[coinsIndex++] = { x: startx, y: starty };
        }
      } else if (element == "?") {
        bonus[bonusIndex++] = { x: startx, y: starty };
      } else if (element == "%") {
        princesPosition_x = startx;
        princesPosition_y = starty;
      }
      startx += tileSize;
    }
    startx = 0;
    starty += tileSize;
  }

  // DRAW WALLS
  drawGameObject(gameObjects.wall, walls, wallsIndex, false);

  // DRAW COINS
  drawGameObject(gameObjects.coin, coins, coinsIndex, true);

  // DRAW BONUS
  drawGameObject(gameObjects.bonus, bonus, bonusIndex, false);

  // DRAW PRINCES
  if (princesPosition_x != null && princesPosition_y != null) {
    ctx.drawImage(
      gameObjects.princes,
      princesPosition_x,
      princesPosition_y - 9
    );
  }

  // DRAW PLAYER
  if (
    gameObjects.player != null &&
    playerPosition_x != null &&
    playerPosition_y != null
  ) {
    if (
      keyboard.right &&
      canGoRight(playerPosition_x, playerPosition_y)
    ) {
      player.x += playerMoveSize;
    } else if (
      keyboard.left &&
      canGoLeft(playerPosition_x, playerPosition_y)
    ) {
      player.x -= playerMoveSize;
    }

    if (fallsDown(playerPosition_x, playerPosition_y) && !isJumping) {
      var fallHeigh = 0;
      while (
        fallHeigh < tileSize / 3 &&
        fallsDown(playerPosition_x, playerPosition_y + fallHeigh)
      ) {
        fallHeigh++;
      }
      player.y += fallHeigh;
    } else if (keyboard.up && !isJumping) {
      isJumping = true;
    }

    if (isJumping) {
      var increment = playerJumpSize / 5;
      jumpDistance += increment;
      if (jumpDistance >= playerJumpSize) {
        isJumping = false;
        jumpDistance = 0;
      } else {
        var x = 0;
        while (
          canGoUp(playerPosition_x, playerPosition_y - 1) &&
          x <= increment
        ) {
          player.y -= 1;
          x++;
        }
      }
    }

    ctx.drawImage(gameObjects.player, playerPosition_x, playerPosition_y);
  }
}

function drawGameObject(gameObj, positions, length, isBlocker) {
  if (gameObj != null) {
    for (var i = 0; i < length; i++) {
      ctx.drawImage(gameObj, positions[i].x, positions[i].y);
      if (isBlocker == false) {
        blockers[blockers.length] = {
          x: positions[i].x,
          y: positions[i].y,
          width: tileSize,
          height: tileSize
        };
      }
    }
  }
}

// =========================================================================
// COLISION DETECTION
// =========================================================================

function collide(r1, r2) {
  return !(
    r2.left > r1.right ||
    r2.right < r1.left ||
    r2.top > r1.bottom ||
    r2.bottom < r1.top
  );
}

function toTileRect(x, y) {
  return {
    left: x,
    right: x + tileSize,
    top: y,
    bottom: y + tileSize
  };
}

function toTileRect2(x, y, size) {
  return {
    left: x,
    right: x + size,
    top: y,
    bottom: y + size
  };
}

function canGoRight(oldX, oldY) {
  for (var i = 0; i < blockers.length; i++) {
    if (
      collide(
        toTileRect(oldX + playerMoveSize, oldY),
        toTileRect(blockers[i].x, blockers[i].y)
      )
    ) {
      return false;
    }
  }
  return true;
}

function canGoLeft(oldX, oldY) {
  for (var i = 0; i < blockers.length; i++) {
    if (
      collide(
        toTileRect(oldX - playerMoveSize-1, oldY),
        toTileRect(blockers[i].x, blockers[i].y)
      )
    ) {
      return false;
    }
  }
  return true;
}

function canGoUp(oldX, oldY) {
  for (var i = 0; i < blockers.length; i++) {
    if (
      collide(
        toTileRect2(oldX+1, oldY - 2, tileSize-1),
        toTileRect(blockers[i].x, blockers[i].y)
      )
    ) {
      return false;
    }
  }
  return true;
}

function fallsDown(player_x, player_y) {
  var new_y = player_y + tileSize + playerVerticalOffset + 1;
  for (var i = 0; i < blockers.length; i++) {
    var onTheGround =
      new_y >= blockers[i].y && new_y <= blockers[i].y + blockers[i].height;
    var rightValid =
      player_x >= blockers[i].x &&
      player_x <= blockers[i].x + blockers[i].width;
    var leftValid =
      player_x + tileSize >= blockers[i].x &&
      player_x + tileSize <= blockers[i].x + blockers[i].width;
    if ((onTheGround && rightValid) || (onTheGround && leftValid)) {
      return false;
    }
  }
  return true;
}
