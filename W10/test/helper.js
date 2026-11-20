var texture;

function translation(dx, dy, dz, gl, program){
    var forMatrix = new Float32Array([
        1.0, 0.0, 0.0, 0.0,
        0.0, 1.0, 0.0, 0.0,
        0.0, 0.0, 1.0, 0.0,
        dx, dy, dz, 1.0
    ]);

    var uFormMatrix = gl.getUniformLocation(program, 'uFormMatrix');
    gl.uniformMatrix4fv(uFormMatrix, false, forMatrix);
}

function scale(sx, sy, sz, gl, program){
    var forMatrix = new Float32Array([
        sx, 0.0, 0.0, 0.0,
        0.0, sy, 0.0, 0.0,
        0.0, 0.0, sz, 0.0,
        0.0, 0.0, 0.0, 1.0
    ]);

    var uFormMatrix = gl.getUniformLocation(program, 'uFormMatrix');
    gl.uniformMatrix4fv(uFormMatrix, false, forMatrix);
}

function shear(angle, gl, program){
    var cota = 1/Math.tan(angle);
    var forMatrix = new Float32Array([
        1.0, cota, 0.0, 0.0,
        0.0, 1.0, 0.0, 0.0,
        0.0, 0.0, 1.0, 0.0,
        0.0, 0.0, 0.0, 1.0
    ]);

    var uFormMatrix = gl.getUniformLocation(program, 'uFormMatrix');
    gl.uniformMatrix4fv(uFormMatrix, false, forMatrix);
}

function rotateZ(angle, gl, program){
    var sa = Math.sin(angle);
    var ca = Math.cos(angle);
    var forMatrix = new Float32Array([
        ca, -sa, 0.0, 0.0,
        sa, ca, 0.0, 0.0,
        0.0, 0.0, 1.0, 0.0,
        0.0, 0.0, 0.0, 1.0
    ]);

    var uFormMatrix = gl.getUniformLocation(program, 'uFormMatrix');
    gl.uniformMatrix4fv(uFormMatrix, false, forMatrix);
}

function rotateX(angle, gl, program){
    var sa = Math.sin(angle);
    var ca = Math.cos(angle);
    var forMatrix = new Float32Array([
        1.0, 0.0, 0.0, 0.0,
        0.0, ca, -sa, 0.0,
        0.0, sa, ca, 0.0,
        0.0, 0.0, 0.0, 1.0
    ]);

    var uFormMatrix = gl.getUniformLocation(program, 'uFormMatrix');
    gl.uniformMatrix4fv(uFormMatrix, false, forMatrix);
}

function rotateY(angle, gl, program){
    var sa = Math.sin(angle);
    var ca = Math.cos(angle);
    var forMatrix = new Float32Array([
        ca, 0.0, sa, 0.0,
        0.0, 1.0, 0.0, 0.0,
        -sa, 0.0, ca, 0.0,
        0.0, 0.0, 0.0, 1.0
    ]);

    var uFormMatrix = gl.getUniformLocation(program, 'uFormMatrix');
    gl.uniformMatrix4fv(uFormMatrix, false, forMatrix);
}

// For texture
function initTextures(callback, gl) {
    texture = gl.createTexture();
    var image = new Image();
    image.onload = function() { loadTexture(image, callback, gl);};
    image.src = "txCrate.bmp";
}
function loadTexture(image, callback, gl) {
    // Flip the image's y axis
    gl.pixelStorei(gl.UNPACK_FLIP_Y_WEBGL, 1);
    // Bind the texture object to the target on GPU
    gl.bindTexture(gl.TEXTURE_2D, texture);
    // Set the texture parameters
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.LINEAR);
    // Set the texture image
    gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGB, gl.RGB, gl.UNSIGNED_BYTE, image);
    gl.generateMipmap(gl.TEXTURE_2D);
    gl.bindTexture(gl.TEXTURE_2D, null);
    callback();
}
