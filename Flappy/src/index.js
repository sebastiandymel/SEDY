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
window.onload = load;
document.addEventListener("keydown", keyDownHandler, false);
document.addEventListener("keyup", keyUpHandler, false);

function load() {
  canvas = document.getElementById("canvas");
  canvas.setAttribute("width", 800);
  canvas.setAttribute("height", 480);
  ctx = canvas.getContext("2d");

  var offset = canvas.width / 5;

  for (var i = 0; i < 5; i++) {
    bars[i] = {
      x: offset * i + canvas.width / 3,
      gap: 30 * (i + 1)
    };
  }

  setInterval(draw, refresh_rate_ms);
}

function draw() {
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
  }

  // DRAW PLAYER
  if (isUp) {
    player_y -= player_speed * 5;
  } else {
    player_y += player_speed;
  }

  ctx.beginPath();
  ctx.arc(100, player_y, player_height / 2, 0, Math.PI * 2);
  ctx.fillStyle = "black";
  ctx.fill();
  ctx.closePath();
}

function keyDownHandler(e) {
  if (e.key == "W" || e.key == "w") {
    isUp = true;
  }
}

function keyUpHandler(e) {
  if (e.key == "W" || e.key == "w") {
    isUp = false;
  }
}
