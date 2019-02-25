import _ from "lodash";

const refresh_rate_ms = 50;
const bar_width = 50;
const bar_speed = 5;
const player_height = 60;
const player_speed = 6;

var canvas = null;
var ctx = null;
var bars = [];
var player_y = 100;
var isUp = false;
var colision = false;
var player_center_x = 100;

window.onload = load;
document.addEventListener("keydown", keyDownHandler, false);
document.addEventListener("keyup", keyUpHandler, false);

function load() {
  canvas = document.getElementById("canvas");
  canvas.setAttribute("width", 800);
  canvas.setAttribute("height", 480);
  ctx = canvas.getContext("2d");

  var numberOfBarsPerScreen = 5;
  var offset = canvas.width / numberOfBarsPerScreen;

  for (var i = 0; i < numberOfBarsPerScreen; i++) {
    bars[i] = {
      x: offset * i + canvas.width / 2,
      gap: 30 * (i + 1)
    };
  }

  setInterval(draw, refresh_rate_ms);
}

function draw() {
  if (colision) {
    return;
  }

  ctx.clearRect(0, 0, canvas.width, canvas.height);

  var gapSize = player_height * 3.5;

  // DRAW BARS
  for (var i = 0; i < bars.length; i++) {
    bars[i].x -= bar_speed;

    if (bars[i].x < -bar_width) {
      bars[i].x = canvas.width;
    }

    var x = bars[i].x;
    var y = 0;
    var height = canvas.height - bars[i].gap - gapSize;
    var width = bar_width;

    ctx.beginPath();
    ctx.rect(x, y, width, height);
    ctx.fillStyle = "green";
    ctx.fill();
    ctx.closePath();

    ctx.beginPath();
    ctx.rect(x, height + gapSize, width, bars[i].gap);
    ctx.fillStyle = "green";
    ctx.fill();
    ctx.closePath();

    var r1 = toRect(x, y, width, height);
    var r2 = toRect(x, height + gapSize, width, bars[i].gap);
    var playerR = playerRect();
    colision = colision || hasCollide(playerR, r1) || hasCollide(playerR, r2);    
  }

  // UPDATE PLAYER POSITION
  if (!colision) {
    if (isUp) {
      player_y -= player_speed * 5;
    } else {
      player_y += player_speed;
    }
  }

  ctx.beginPath();
  ctx.arc(player_center_x, player_y, player_height / 2, 0, Math.PI * 2);
  ctx.fillStyle = "black";
  ctx.fill();
  ctx.closePath();
}

function keyDownHandler(e) {
  if (isKeyUp(e)) {
    isUp = true;
  }
}

function keyUpHandler(e) {
  if (isKeyUp(e)) {
    isUp = false;
  }
}

const spaceKeycode = 32;
function isKeyUp(e) {
  if (e.key == "W" || e.key == "w" || e.keyCode == spaceKeycode) {
    return true;
  }
  return false;
}

function hasCollide(r1, r2) {
  return !(
    r2.left > r1.right ||
    r2.right < r1.left ||
    r2.top > r1.bottom ||
    r2.bottom < r1.top
  );
}

function playerRect() {
  return toRect(
    player_center_x - player_height / 2,
    player_y - player_height / 2,
    player_height,
    player_height
  );
}

function toRect(x, y, width, height) {
  return {
    left: x,
    right: x + width,
    top: y,
    bottom: y + height
  };
}
