function main() {
    /**
     * @type {HTMLCanvasElement}
     * */
    var canvas = document.getElementById("myCanvas")

    var gl = canvas.getContext("webgl")

    var vertices = [
        0.1, 0.5, 1.0, 0.0, 0.0, // A red
        0.5, 0.5, 0.0, 1.0, 0.0, // B green
        0.5, 0.1, 0.0, 0.0, 1.0, // C blue
        0.1, 0.1, 1.0, 0.0, 1.0 // D purple
    ]

    var positionBuffer = gl.createBuffer()
    gl.bindBuffer(gl.ARRAY_BUFFER, positionBuffer)
    gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(vertices), gl.STATIC_DRAW)
    gl.bindBuffer(gl.ARRAY_BUFFER, null)



    var vertexShaderCode = `
    attribute vec2 aPosition;
    attribute vec3 aColor;
    varying vec3 vColor;
    uniform vec4 translation;
    void main(){
        vColor = aColor;
        gl_Position = vec4(aPosition, 0.0, 1.0) + translation;
        // gl_PointSize = 20.0;
    }
    `;

    var fragmentShaderCode = `
        precision mediump float;
        varying vec3 vColor;
        void main() {
            gl_FragColor = vec4(vColor, 1.0);
        }
    `;

    var vertexShader = gl.createShader(gl.VERTEX_SHADER);

    gl.shaderSource(vertexShader, vertexShaderCode);
    gl.compileShader(vertexShader);

    if (!gl.getShaderParameter(vertexShader, gl.COMPILE_STATUS)) {
        console.error(gl.getShaderInfoLog(vertexShader));
    }


    var fragmentShader = gl.createShader(gl.FRAGMENT_SHADER);

    gl.shaderSource(fragmentShader, fragmentShaderCode);
    gl.compileShader(fragmentShader);

    if (!gl.getShaderParameter(fragmentShader, gl.COMPILE_STATUS)) {
        console.error(gl.getShaderInfoLog(fragmentShader));
    }


    var program = gl.createProgram();
    gl.attachShader(program, vertexShader)
    gl.attachShader(program, fragmentShader)
    gl.linkProgram(program)
    if (!gl.getProgramParameter(program, gl.LINK_STATUS)) {
        console.error(gl.getProgramInfoLog(program));
    }

    gl.useProgram(program)

    gl.bindBuffer(gl.ARRAY_BUFFER, positionBuffer)
    var aPosition = gl.getAttribLocation(program, "aPosition")
    gl.vertexAttribPointer(aPosition, 2, gl.FLOAT, false, 5 * Float32Array.BYTES_PER_ELEMENT, 0)
    gl.enableVertexAttribArray(aPosition)

    var aColor = gl.getAttribLocation(program, "aColor")
    gl.vertexAttribPointer(aColor, 3, gl.FLOAT, false, 5 * Float32Array.BYTES_PER_ELEMENT, 2 * Float32Array.BYTES_PER_ELEMENT)
    gl.enableVertexAttribArray(aColor)

    var dx = -0.5, dy = 0.1, dz = 0.0;
    var translation = gl.getUniformLocation(program, 'translation')
    gl.uniform4f(translation, dx, dy, dz, 0.0)

    gl.clearColor(1.0, 1.0, 1.0, 1.0)
    gl.clear(gl.COLOR_BUFFER_BIT)
    gl.drawArrays(gl.TRIANGLE_FAN, 0, vertices.length / 5)
}

window.onload = main()
