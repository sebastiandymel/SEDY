/******/ (function(modules) { // webpackBootstrap
/******/ 	// The module cache
/******/ 	var installedModules = {};
/******/
/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {
/******/
/******/ 		// Check if module is in cache
/******/ 		if(installedModules[moduleId]) {
/******/ 			return installedModules[moduleId].exports;
/******/ 		}
/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = installedModules[moduleId] = {
/******/ 			i: moduleId,
/******/ 			l: false,
/******/ 			exports: {}
/******/ 		};
/******/
/******/ 		// Execute the module function
/******/ 		modules[moduleId].call(module.exports, module, module.exports, __webpack_require__);
/******/
/******/ 		// Flag the module as loaded
/******/ 		module.l = true;
/******/
/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}
/******/
/******/
/******/ 	// expose the modules object (__webpack_modules__)
/******/ 	__webpack_require__.m = modules;
/******/
/******/ 	// expose the module cache
/******/ 	__webpack_require__.c = installedModules;
/******/
/******/ 	// define getter function for harmony exports
/******/ 	__webpack_require__.d = function(exports, name, getter) {
/******/ 		if(!__webpack_require__.o(exports, name)) {
/******/ 			Object.defineProperty(exports, name, { enumerable: true, get: getter });
/******/ 		}
/******/ 	};
/******/
/******/ 	// define __esModule on exports
/******/ 	__webpack_require__.r = function(exports) {
/******/ 		if(typeof Symbol !== 'undefined' && Symbol.toStringTag) {
/******/ 			Object.defineProperty(exports, Symbol.toStringTag, { value: 'Module' });
/******/ 		}
/******/ 		Object.defineProperty(exports, '__esModule', { value: true });
/******/ 	};
/******/
/******/ 	// create a fake namespace object
/******/ 	// mode & 1: value is a module id, require it
/******/ 	// mode & 2: merge all properties of value into the ns
/******/ 	// mode & 4: return value when already ns object
/******/ 	// mode & 8|1: behave like require
/******/ 	__webpack_require__.t = function(value, mode) {
/******/ 		if(mode & 1) value = __webpack_require__(value);
/******/ 		if(mode & 8) return value;
/******/ 		if((mode & 4) && typeof value === 'object' && value && value.__esModule) return value;
/******/ 		var ns = Object.create(null);
/******/ 		__webpack_require__.r(ns);
/******/ 		Object.defineProperty(ns, 'default', { enumerable: true, value: value });
/******/ 		if(mode & 2 && typeof value != 'string') for(var key in value) __webpack_require__.d(ns, key, function(key) { return value[key]; }.bind(null, key));
/******/ 		return ns;
/******/ 	};
/******/
/******/ 	// getDefaultExport function for compatibility with non-harmony modules
/******/ 	__webpack_require__.n = function(module) {
/******/ 		var getter = module && module.__esModule ?
/******/ 			function getDefault() { return module['default']; } :
/******/ 			function getModuleExports() { return module; };
/******/ 		__webpack_require__.d(getter, 'a', getter);
/******/ 		return getter;
/******/ 	};
/******/
/******/ 	// Object.prototype.hasOwnProperty.call
/******/ 	__webpack_require__.o = function(object, property) { return Object.prototype.hasOwnProperty.call(object, property); };
/******/
/******/ 	// __webpack_public_path__
/******/ 	__webpack_require__.p = "";
/******/
/******/
/******/ 	// Load entry module and return exports
/******/ 	return __webpack_require__(__webpack_require__.s = "./src/index.js");
/******/ })
/************************************************************************/
/******/ ({

/***/ "./node_modules/lodash/lodash.js":
/*!***************************************!*\
  !*** ./node_modules/lodash/lodash.js ***!
  \***************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {


/***/ }),

/***/ "./node_modules/webpack/buildin/global.js":
/*!***********************************!*\
  !*** (webpack)/buildin/global.js ***!
  \***********************************/
/*! no static exports found */
/***/ (function(module, exports) {

eval("var g;\n\n// This works in non-strict mode\ng = (function() {\n\treturn this;\n})();\n\ntry {\n\t// This works if eval is allowed (see CSP)\n\tg = g || new Function(\"return this\")();\n} catch (e) {\n\t// This works if the window reference is available\n\tif (typeof window === \"object\") g = window;\n}\n\n// g can still be undefined, but nothing to do about it...\n// We return undefined, instead of nothing here, so it's\n// easier to handle this case. if(!global) { ...}\n\nmodule.exports = g;\n//# sourceURL=[module]\n//# sourceMappingURL=data:application/json;charset=utf-8;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiLi9ub2RlX21vZHVsZXMvd2VicGFjay9idWlsZGluL2dsb2JhbC5qcy5qcyIsInNvdXJjZXMiOlsid2VicGFjazovLy8od2VicGFjaykvYnVpbGRpbi9nbG9iYWwuanM/Y2QwMCJdLCJzb3VyY2VzQ29udGVudCI6WyJ2YXIgZztcblxuLy8gVGhpcyB3b3JrcyBpbiBub24tc3RyaWN0IG1vZGVcbmcgPSAoZnVuY3Rpb24oKSB7XG5cdHJldHVybiB0aGlzO1xufSkoKTtcblxudHJ5IHtcblx0Ly8gVGhpcyB3b3JrcyBpZiBldmFsIGlzIGFsbG93ZWQgKHNlZSBDU1ApXG5cdGcgPSBnIHx8IG5ldyBGdW5jdGlvbihcInJldHVybiB0aGlzXCIpKCk7XG59IGNhdGNoIChlKSB7XG5cdC8vIFRoaXMgd29ya3MgaWYgdGhlIHdpbmRvdyByZWZlcmVuY2UgaXMgYXZhaWxhYmxlXG5cdGlmICh0eXBlb2Ygd2luZG93ID09PSBcIm9iamVjdFwiKSBnID0gd2luZG93O1xufVxuXG4vLyBnIGNhbiBzdGlsbCBiZSB1bmRlZmluZWQsIGJ1dCBub3RoaW5nIHRvIGRvIGFib3V0IGl0Li4uXG4vLyBXZSByZXR1cm4gdW5kZWZpbmVkLCBpbnN0ZWFkIG9mIG5vdGhpbmcgaGVyZSwgc28gaXQnc1xuLy8gZWFzaWVyIHRvIGhhbmRsZSB0aGlzIGNhc2UuIGlmKCFnbG9iYWwpIHsgLi4ufVxuXG5tb2R1bGUuZXhwb3J0cyA9IGc7XG4iXSwibWFwcGluZ3MiOiJBQUFBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7Iiwic291cmNlUm9vdCI6IiJ9\n//# sourceURL=webpack-internal:///./node_modules/webpack/buildin/global.js\n");

/***/ }),

/***/ "./node_modules/webpack/buildin/module.js":
/*!***********************************!*\
  !*** (webpack)/buildin/module.js ***!
  \***********************************/
/*! no static exports found */
/***/ (function(module, exports) {

eval("module.exports = function(module) {\n\tif (!module.webpackPolyfill) {\n\t\tmodule.deprecate = function() {};\n\t\tmodule.paths = [];\n\t\t// module.parent = undefined by default\n\t\tif (!module.children) module.children = [];\n\t\tObject.defineProperty(module, \"loaded\", {\n\t\t\tenumerable: true,\n\t\t\tget: function() {\n\t\t\t\treturn module.l;\n\t\t\t}\n\t\t});\n\t\tObject.defineProperty(module, \"id\", {\n\t\t\tenumerable: true,\n\t\t\tget: function() {\n\t\t\t\treturn module.i;\n\t\t\t}\n\t\t});\n\t\tmodule.webpackPolyfill = 1;\n\t}\n\treturn module;\n};\n//# sourceURL=[module]\n//# sourceMappingURL=data:application/json;charset=utf-8;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiLi9ub2RlX21vZHVsZXMvd2VicGFjay9idWlsZGluL21vZHVsZS5qcy5qcyIsInNvdXJjZXMiOlsid2VicGFjazovLy8od2VicGFjaykvYnVpbGRpbi9tb2R1bGUuanM/Y2VkMiJdLCJzb3VyY2VzQ29udGVudCI6WyJtb2R1bGUuZXhwb3J0cyA9IGZ1bmN0aW9uKG1vZHVsZSkge1xuXHRpZiAoIW1vZHVsZS53ZWJwYWNrUG9seWZpbGwpIHtcblx0XHRtb2R1bGUuZGVwcmVjYXRlID0gZnVuY3Rpb24oKSB7fTtcblx0XHRtb2R1bGUucGF0aHMgPSBbXTtcblx0XHQvLyBtb2R1bGUucGFyZW50ID0gdW5kZWZpbmVkIGJ5IGRlZmF1bHRcblx0XHRpZiAoIW1vZHVsZS5jaGlsZHJlbikgbW9kdWxlLmNoaWxkcmVuID0gW107XG5cdFx0T2JqZWN0LmRlZmluZVByb3BlcnR5KG1vZHVsZSwgXCJsb2FkZWRcIiwge1xuXHRcdFx0ZW51bWVyYWJsZTogdHJ1ZSxcblx0XHRcdGdldDogZnVuY3Rpb24oKSB7XG5cdFx0XHRcdHJldHVybiBtb2R1bGUubDtcblx0XHRcdH1cblx0XHR9KTtcblx0XHRPYmplY3QuZGVmaW5lUHJvcGVydHkobW9kdWxlLCBcImlkXCIsIHtcblx0XHRcdGVudW1lcmFibGU6IHRydWUsXG5cdFx0XHRnZXQ6IGZ1bmN0aW9uKCkge1xuXHRcdFx0XHRyZXR1cm4gbW9kdWxlLmk7XG5cdFx0XHR9XG5cdFx0fSk7XG5cdFx0bW9kdWxlLndlYnBhY2tQb2x5ZmlsbCA9IDE7XG5cdH1cblx0cmV0dXJuIG1vZHVsZTtcbn07XG4iXSwibWFwcGluZ3MiOiJBQUFBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBOyIsInNvdXJjZVJvb3QiOiIifQ==\n//# sourceURL=webpack-internal:///./node_modules/webpack/buildin/module.js\n");

/***/ }),

/***/ "./src/index.js":
/*!**********************!*\
  !*** ./src/index.js ***!
  \**********************/
/*! no exports provided */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
eval("__webpack_require__.r(__webpack_exports__);\n/* harmony import */ var lodash__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! lodash */ \"./node_modules/lodash/lodash.js\");\n/* harmony import */ var lodash__WEBPACK_IMPORTED_MODULE_0___default = /*#__PURE__*/__webpack_require__.n(lodash__WEBPACK_IMPORTED_MODULE_0__);\n\r\n\r\nconst refresh_rate_ms = 50;\r\nconst bar_width = 50;\r\nconst bar_speed = 5;\r\nconst player_height = 60;\r\nconst player_speed = 6;\r\n\r\nvar canvas = null;\r\nvar ctx = null;\r\nvar bars = [];\r\nvar player_y = 100;\r\nvar isUp = false;\r\nvar colision = false;\r\nvar player_center_x = 100;\r\n\r\nwindow.onload = load;\r\ndocument.addEventListener(\"keydown\", keyDownHandler, false);\r\ndocument.addEventListener(\"keyup\", keyUpHandler, false);\r\n\r\nfunction load() {\r\n  canvas = document.getElementById(\"canvas\");\r\n  canvas.setAttribute(\"width\", 800);\r\n  canvas.setAttribute(\"height\", 480);\r\n  ctx = canvas.getContext(\"2d\");\r\n\r\n  var numberOfBarsPerScreen = 5;\r\n  var offset = canvas.width / numberOfBarsPerScreen;\r\n\r\n  for (var i = 0; i < numberOfBarsPerScreen; i++) {\r\n    bars[i] = {\r\n      x: offset * i + canvas.width / 2,\r\n      gap: 30 * (i + 1)\r\n    };\r\n  }\r\n\r\n  setInterval(draw, refresh_rate_ms);\r\n}\r\n\r\nfunction draw() {\r\n  if (colision) {\r\n    return;\r\n  }\r\n\r\n  ctx.clearRect(0, 0, canvas.width, canvas.height);\r\n\r\n  var gapSize = player_height * 3.5;\r\n\r\n  // DRAW BARS\r\n  for (var i = 0; i < bars.length; i++) {\r\n    bars[i].x -= bar_speed;\r\n\r\n    if (bars[i].x < -bar_width) {\r\n      bars[i].x = canvas.width;\r\n    }\r\n\r\n    var x = bars[i].x;\r\n    var y = 0;\r\n    var height = canvas.height - bars[i].gap - gapSize;\r\n    var width = bar_width;\r\n\r\n    ctx.beginPath();\r\n    ctx.rect(x, y, width, height);\r\n    ctx.fillStyle = \"green\";\r\n    ctx.fill();\r\n    ctx.closePath();\r\n\r\n    ctx.beginPath();\r\n    ctx.rect(x, height + gapSize, width, bars[i].gap);\r\n    ctx.fillStyle = \"green\";\r\n    ctx.fill();\r\n    ctx.closePath();\r\n\r\n    var r1 = toRect(x, y, width, height);\r\n    var r2 = toRect(x, height + gapSize, width, bars[i].gap);\r\n    var playerR = playerRect();\r\n    colision = colision || hasCollide(playerR, r1) || hasCollide(playerR, r2);    \r\n  }\r\n\r\n  // UPDATE PLAYER POSITION\r\n  if (!colision) {\r\n    if (isUp) {\r\n      player_y -= player_speed * 5;\r\n    } else {\r\n      player_y += player_speed;\r\n    }\r\n  }\r\n\r\n  ctx.beginPath();\r\n  ctx.arc(player_center_x, player_y, player_height / 2, 0, Math.PI * 2);\r\n  ctx.fillStyle = \"black\";\r\n  ctx.fill();\r\n  ctx.closePath();\r\n}\r\n\r\nfunction keyDownHandler(e) {\r\n  if (isKeyUp(e)) {\r\n    isUp = true;\r\n  }\r\n}\r\n\r\nfunction keyUpHandler(e) {\r\n  if (isKeyUp(e)) {\r\n    isUp = false;\r\n  }\r\n}\r\n\r\nconst spaceKeycode = 32;\r\nfunction isKeyUp(e) {\r\n  if (e.key == \"W\" || e.key == \"w\" || e.keyCode == spaceKeycode) {\r\n    return true;\r\n  }\r\n  return false;\r\n}\r\n\r\nfunction hasCollide(r1, r2) {\r\n  return !(\r\n    r2.left > r1.right ||\r\n    r2.right < r1.left ||\r\n    r2.top > r1.bottom ||\r\n    r2.bottom < r1.top\r\n  );\r\n}\r\n\r\nfunction playerRect() {\r\n  return toRect(\r\n    player_center_x - player_height / 2,\r\n    player_y - player_height / 2,\r\n    player_height,\r\n    player_height\r\n  );\r\n}\r\n\r\nfunction toRect(x, y, width, height) {\r\n  return {\r\n    left: x,\r\n    right: x + width,\r\n    top: y,\r\n    bottom: y + height\r\n  };\r\n}\r\n//# sourceURL=[module]\n//# sourceMappingURL=data:application/json;charset=utf-8;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiLi9zcmMvaW5kZXguanMuanMiLCJzb3VyY2VzIjpbIndlYnBhY2s6Ly8vLi9zcmMvaW5kZXguanM/YjYzNSJdLCJzb3VyY2VzQ29udGVudCI6WyJpbXBvcnQgXyBmcm9tIFwibG9kYXNoXCI7XHJcblxyXG5jb25zdCByZWZyZXNoX3JhdGVfbXMgPSA1MDtcclxuY29uc3QgYmFyX3dpZHRoID0gNTA7XHJcbmNvbnN0IGJhcl9zcGVlZCA9IDU7XHJcbmNvbnN0IHBsYXllcl9oZWlnaHQgPSA2MDtcclxuY29uc3QgcGxheWVyX3NwZWVkID0gNjtcclxuXHJcbnZhciBjYW52YXMgPSBudWxsO1xyXG52YXIgY3R4ID0gbnVsbDtcclxudmFyIGJhcnMgPSBbXTtcclxudmFyIHBsYXllcl95ID0gMTAwO1xyXG52YXIgaXNVcCA9IGZhbHNlO1xyXG52YXIgY29saXNpb24gPSBmYWxzZTtcclxudmFyIHBsYXllcl9jZW50ZXJfeCA9IDEwMDtcclxuXHJcbndpbmRvdy5vbmxvYWQgPSBsb2FkO1xyXG5kb2N1bWVudC5hZGRFdmVudExpc3RlbmVyKFwia2V5ZG93blwiLCBrZXlEb3duSGFuZGxlciwgZmFsc2UpO1xyXG5kb2N1bWVudC5hZGRFdmVudExpc3RlbmVyKFwia2V5dXBcIiwga2V5VXBIYW5kbGVyLCBmYWxzZSk7XHJcblxyXG5mdW5jdGlvbiBsb2FkKCkge1xyXG4gIGNhbnZhcyA9IGRvY3VtZW50LmdldEVsZW1lbnRCeUlkKFwiY2FudmFzXCIpO1xyXG4gIGNhbnZhcy5zZXRBdHRyaWJ1dGUoXCJ3aWR0aFwiLCA4MDApO1xyXG4gIGNhbnZhcy5zZXRBdHRyaWJ1dGUoXCJoZWlnaHRcIiwgNDgwKTtcclxuICBjdHggPSBjYW52YXMuZ2V0Q29udGV4dChcIjJkXCIpO1xyXG5cclxuICB2YXIgbnVtYmVyT2ZCYXJzUGVyU2NyZWVuID0gNTtcclxuICB2YXIgb2Zmc2V0ID0gY2FudmFzLndpZHRoIC8gbnVtYmVyT2ZCYXJzUGVyU2NyZWVuO1xyXG5cclxuICBmb3IgKHZhciBpID0gMDsgaSA8IG51bWJlck9mQmFyc1BlclNjcmVlbjsgaSsrKSB7XHJcbiAgICBiYXJzW2ldID0ge1xyXG4gICAgICB4OiBvZmZzZXQgKiBpICsgY2FudmFzLndpZHRoIC8gMixcclxuICAgICAgZ2FwOiAzMCAqIChpICsgMSlcclxuICAgIH07XHJcbiAgfVxyXG5cclxuICBzZXRJbnRlcnZhbChkcmF3LCByZWZyZXNoX3JhdGVfbXMpO1xyXG59XHJcblxyXG5mdW5jdGlvbiBkcmF3KCkge1xyXG4gIGlmIChjb2xpc2lvbikge1xyXG4gICAgcmV0dXJuO1xyXG4gIH1cclxuXHJcbiAgY3R4LmNsZWFyUmVjdCgwLCAwLCBjYW52YXMud2lkdGgsIGNhbnZhcy5oZWlnaHQpO1xyXG5cclxuICB2YXIgZ2FwU2l6ZSA9IHBsYXllcl9oZWlnaHQgKiAzLjU7XHJcblxyXG4gIC8vIERSQVcgQkFSU1xyXG4gIGZvciAodmFyIGkgPSAwOyBpIDwgYmFycy5sZW5ndGg7IGkrKykge1xyXG4gICAgYmFyc1tpXS54IC09IGJhcl9zcGVlZDtcclxuXHJcbiAgICBpZiAoYmFyc1tpXS54IDwgLWJhcl93aWR0aCkge1xyXG4gICAgICBiYXJzW2ldLnggPSBjYW52YXMud2lkdGg7XHJcbiAgICB9XHJcblxyXG4gICAgdmFyIHggPSBiYXJzW2ldLng7XHJcbiAgICB2YXIgeSA9IDA7XHJcbiAgICB2YXIgaGVpZ2h0ID0gY2FudmFzLmhlaWdodCAtIGJhcnNbaV0uZ2FwIC0gZ2FwU2l6ZTtcclxuICAgIHZhciB3aWR0aCA9IGJhcl93aWR0aDtcclxuXHJcbiAgICBjdHguYmVnaW5QYXRoKCk7XHJcbiAgICBjdHgucmVjdCh4LCB5LCB3aWR0aCwgaGVpZ2h0KTtcclxuICAgIGN0eC5maWxsU3R5bGUgPSBcImdyZWVuXCI7XHJcbiAgICBjdHguZmlsbCgpO1xyXG4gICAgY3R4LmNsb3NlUGF0aCgpO1xyXG5cclxuICAgIGN0eC5iZWdpblBhdGgoKTtcclxuICAgIGN0eC5yZWN0KHgsIGhlaWdodCArIGdhcFNpemUsIHdpZHRoLCBiYXJzW2ldLmdhcCk7XHJcbiAgICBjdHguZmlsbFN0eWxlID0gXCJncmVlblwiO1xyXG4gICAgY3R4LmZpbGwoKTtcclxuICAgIGN0eC5jbG9zZVBhdGgoKTtcclxuXHJcbiAgICB2YXIgcjEgPSB0b1JlY3QoeCwgeSwgd2lkdGgsIGhlaWdodCk7XHJcbiAgICB2YXIgcjIgPSB0b1JlY3QoeCwgaGVpZ2h0ICsgZ2FwU2l6ZSwgd2lkdGgsIGJhcnNbaV0uZ2FwKTtcclxuICAgIHZhciBwbGF5ZXJSID0gcGxheWVyUmVjdCgpO1xyXG4gICAgY29saXNpb24gPSBjb2xpc2lvbiB8fCBoYXNDb2xsaWRlKHBsYXllclIsIHIxKSB8fCBoYXNDb2xsaWRlKHBsYXllclIsIHIyKTsgICAgXHJcbiAgfVxyXG5cclxuICAvLyBVUERBVEUgUExBWUVSIFBPU0lUSU9OXHJcbiAgaWYgKCFjb2xpc2lvbikge1xyXG4gICAgaWYgKGlzVXApIHtcclxuICAgICAgcGxheWVyX3kgLT0gcGxheWVyX3NwZWVkICogNTtcclxuICAgIH0gZWxzZSB7XHJcbiAgICAgIHBsYXllcl95ICs9IHBsYXllcl9zcGVlZDtcclxuICAgIH1cclxuICB9XHJcblxyXG4gIGN0eC5iZWdpblBhdGgoKTtcclxuICBjdHguYXJjKHBsYXllcl9jZW50ZXJfeCwgcGxheWVyX3ksIHBsYXllcl9oZWlnaHQgLyAyLCAwLCBNYXRoLlBJICogMik7XHJcbiAgY3R4LmZpbGxTdHlsZSA9IFwiYmxhY2tcIjtcclxuICBjdHguZmlsbCgpO1xyXG4gIGN0eC5jbG9zZVBhdGgoKTtcclxufVxyXG5cclxuZnVuY3Rpb24ga2V5RG93bkhhbmRsZXIoZSkge1xyXG4gIGlmIChpc0tleVVwKGUpKSB7XHJcbiAgICBpc1VwID0gdHJ1ZTtcclxuICB9XHJcbn1cclxuXHJcbmZ1bmN0aW9uIGtleVVwSGFuZGxlcihlKSB7XHJcbiAgaWYgKGlzS2V5VXAoZSkpIHtcclxuICAgIGlzVXAgPSBmYWxzZTtcclxuICB9XHJcbn1cclxuXHJcbmNvbnN0IHNwYWNlS2V5Y29kZSA9IDMyO1xyXG5mdW5jdGlvbiBpc0tleVVwKGUpIHtcclxuICBpZiAoZS5rZXkgPT0gXCJXXCIgfHwgZS5rZXkgPT0gXCJ3XCIgfHwgZS5rZXlDb2RlID09IHNwYWNlS2V5Y29kZSkge1xyXG4gICAgcmV0dXJuIHRydWU7XHJcbiAgfVxyXG4gIHJldHVybiBmYWxzZTtcclxufVxyXG5cclxuZnVuY3Rpb24gaGFzQ29sbGlkZShyMSwgcjIpIHtcclxuICByZXR1cm4gIShcclxuICAgIHIyLmxlZnQgPiByMS5yaWdodCB8fFxyXG4gICAgcjIucmlnaHQgPCByMS5sZWZ0IHx8XHJcbiAgICByMi50b3AgPiByMS5ib3R0b20gfHxcclxuICAgIHIyLmJvdHRvbSA8IHIxLnRvcFxyXG4gICk7XHJcbn1cclxuXHJcbmZ1bmN0aW9uIHBsYXllclJlY3QoKSB7XHJcbiAgcmV0dXJuIHRvUmVjdChcclxuICAgIHBsYXllcl9jZW50ZXJfeCAtIHBsYXllcl9oZWlnaHQgLyAyLFxyXG4gICAgcGxheWVyX3kgLSBwbGF5ZXJfaGVpZ2h0IC8gMixcclxuICAgIHBsYXllcl9oZWlnaHQsXHJcbiAgICBwbGF5ZXJfaGVpZ2h0XHJcbiAgKTtcclxufVxyXG5cclxuZnVuY3Rpb24gdG9SZWN0KHgsIHksIHdpZHRoLCBoZWlnaHQpIHtcclxuICByZXR1cm4ge1xyXG4gICAgbGVmdDogeCxcclxuICAgIHJpZ2h0OiB4ICsgd2lkdGgsXHJcbiAgICB0b3A6IHksXHJcbiAgICBib3R0b206IHkgKyBoZWlnaHRcclxuICB9O1xyXG59XHJcbiJdLCJtYXBwaW5ncyI6IkFBQUE7QUFBQTtBQUFBO0FBQUE7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBOyIsInNvdXJjZVJvb3QiOiIifQ==\n//# sourceURL=webpack-internal:///./src/index.js\n");

/***/ })

/******/ });