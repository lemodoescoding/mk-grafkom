/**
 * @returns {void}
 */
function main() {
    /**
     * @type {HTMLCanvasElement}
     */
    var canvas = document.getElementById("myCanvas");
    var gl = canvas.getContext("webgl");

    if (!gl) {
        console.error("WebGL tidak didukung pada browser ini.");
        return;
    }

    var sTop = createChamferedRectangle(
        0.32,   // width
        0.16,   // height
        0.06,   // diagonal
        0.165,   // x
        0.32    // y
    );

    var sTop2 = createChamferedRectangle(
        0.33,   // width
        0.16,   // height
        0.06,   // diagonal
        -0.16,   // x
        0.32,   // y
        true,
        false
    );

    var sUpperLeft = createChamferedRectangle(
        0.16,
        0.30,
        0.04,
        -0.245,
        0.19,
        true
    );

    var sMiddle = createChamferedRectangle(
        0.32,
        0.16,
        0.06,
        -0.165,
        0.00,
        true,
        true
    );

    var sMiddle2 = createChamferedRectangle(
        0.33,
        0.16,
        0.06,
        0.16,
        0.00,
        false,
        false
    );

    var sLowerRight = createChamferedRectangle(
        0.16,
        0.30,
        0.04,
        0.245,
        -0.19,
        false,
        true
    );

    var sBottom = createChamferedRectangle(
        0.32,
        0.16,
        0.06,
        0.165,
        -0.32,
        false,
        true
    );

    var sBottom2 = createChamferedRectangle(
        0.33,
        0.16,
        0.06,
        -0.16,
        -0.32,
        true,
        true
    );

    var tMiddle = createChamferedRectangle(
        0.16,
        0.30,
        0.04,
        0.245,
        -0.19,
        false,
        true
    )

    var tTop = createChamferedRectangle(
        0.32,   // width
        0.20,   // height
        0.06,   // diagonal
        0.715,   // x
        0.32    // y
    );

    var tTop2 = createChamferedRectangle(
        0.33,   // width
        0.20,   // height
        0.06,   // diagonal
        0.39,   // x
        0.32,   // y
        true,
        false
    );

    var tStem = createChamferedRectangle(
        0.16,   // width
        0.48,   // height
        0.04,   // diagonal
        0.55,   // x
        0.00    // y
    );

    var tStem = createRectangle(
        0.13,   // width
        0.80,   // height
        0.55,   // x
        0.00    // y
    );

    var verticesT = combineShapes(
        tTop,
        tTop2,
        tStem
    );

    var verticesS = combineShapes(
        sTop,
        sTop2,
        sUpperLeft,
        sMiddle,
        sMiddle2,
        sLowerRight,
        sBottom,
        sBottom2
    );


    verticesS = transformX(verticesS, -0.45);
    verticesT = transformX(verticesT, -0.20)

    var vertices = combineShapes(
        verticesS,
        verticesT
    );

    var dVertexCount = vertices.length / 3;

    var positionBuffer = gl.createBuffer();
    gl.bindBuffer(gl.ARRAY_BUFFER, positionBuffer);
    gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(vertices), gl.STATIC_DRAW);
    gl.bindBuffer(gl.ARRAY_BUFFER, null);

    var vertexShaderCode = `
        attribute vec3 aPosition;
        void main(){
            gl_Position = vec4(aPosition, 1.0);
        }`;

    var fragmentShaderCode = `
        precision mediump float;
        uniform vec4 uColor;
        void main(){
            gl_FragColor = uColor;
        }`;

    var vertexShader = gl.createShader(gl.VERTEX_SHADER);
    gl.shaderSource(vertexShader, vertexShaderCode);
    gl.compileShader(vertexShader);
    if (!gl.getShaderParameter(vertexShader, gl.COMPILE_STATUS)) {
        console.error("Vertex shader error:", gl.getShaderInfoLog(vertexShader));
    }

    var fragmentShader = gl.createShader(gl.FRAGMENT_SHADER);
    gl.shaderSource(fragmentShader, fragmentShaderCode);
    gl.compileShader(fragmentShader);
    if (!gl.getShaderParameter(fragmentShader, gl.COMPILE_STATUS)) {
        console.error("Fragment shader error:", gl.getShaderInfoLog(fragmentShader));
    }

    var program = gl.createProgram();
    gl.attachShader(program, vertexShader);
    gl.attachShader(program, fragmentShader);
    gl.linkProgram(program);
    gl.useProgram(program);

    gl.bindBuffer(gl.ARRAY_BUFFER, positionBuffer);
    var aPosition = gl.getAttribLocation(program, "aPosition");
    gl.vertexAttribPointer(aPosition, 3, gl.FLOAT, false, 0, 0);
    gl.enableVertexAttribArray(aPosition);

    var uColor = gl.getUniformLocation(program, "uColor");

    gl.clearColor(1.0, 1.0, 1.0, 1.0);
    gl.clear(gl.COLOR_BUFFER_BIT);

    // Huruf "S"
    gl.uniform4f(uColor, 0.0, 0.2, 0.9, 1.0);
    gl.drawArrays(gl.TRIANGLES, 0, dVertexCount);
}
2
window.onload = main;