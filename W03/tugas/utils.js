/**
 * Creates a chamfered rectangle.
 *
 * @param {number} width
 * @param {number} height
 * @param {number} diagonal
 * @param {number} x
 * @param {number} y
 * @param {boolean} flipX
 * @param {boolean} flipY
 * @returns {Array<number>}
 */
function createChamferedRectangle(
    width,
    height,
    diagonal,
    x = 0,
    y = 0,
    flipX = false,
    flipY = false
) {
    var w = width / 2;
    var h = height / 2;

    var points = [
        [-w, h],                    // A
        [w - diagonal, h],          // B
        [w, h - diagonal],          // C
        [w, -h],                    // D
        [-w, -h],                   // E
    ];

    function transform(point) {
        var px = point[0];
        var py = point[1];

        if (flipX) {
            px = -px;
        }

        if (flipY) {
            py = -py;
        }

        return [
            px + x,
            py + y
        ];
    }

    points = points.map(transform);

    var A = points[0];
    var B = points[1];
    var C = points[2];
    var D = points[3];
    var E = points[4];

    return [
        // Triangle 1
        A[0], A[1], 0.0,
        B[0], B[1], 0.0,
        C[0], C[1], 0.0,

        // Triangle 2
        A[0], A[1], 0.0,
        C[0], C[1], 0.0,
        D[0], D[1], 0.0,

        // Triangle 3
        A[0], A[1], 0.0,
        D[0], D[1], 0.0,
        E[0], E[1], 0.0
    ];
}

/**
 * @param {Array<number>} vertices
 * @returns {Array<number>}
 */
function mirrorHorizontal(vertices) {
    return vertices.map((value, index) => {
        return index % 3 === 0 ? -value : value;
    });
}

/**
 * @param {Array<number>} vertices
 * @returns {Array<number>}
 */
function mirrorVertical(vertices) {
    return vertices.map((value, index) => {
        return index % 3 === 1 ? -value : value;
    });
}

/**
 * @param {Array<number>} vertices
 * @returns {Array<number>}
 */
function mirrorBoth(vertices) {
    return vertices.map((value, index) => {
        if (index % 3 === 0) return -value; // X
        if (index % 3 === 1) return -value; // Y
        return value;                       // Z
    });
}

/**
 * 
 * @param {Array<number>} vertices 
 * @param {number} scale 
 * @returns {Array<number>}
 */
function scaleShape(vertices, scale) {
    return vertices.map((value, index) => {
        // X or Y coordinate
        if (index % 3 === 0 || index % 3 === 1) {
            return value * scale;
        }

        // Z coordinate
        return value;
    });
}

/**
 * 
 * @param {number} width 
 * @param {number} height 
 * @param {number} diagonal 
 * @param {number} extensionHeight 
 * @param {number} transformX 
 * @param {number} transformY 
 * @param {number} scale 
 * @param {boolean} flipX 
 * @param {boolean} flipY 
 * @returns {Array<number>}
 */
function createDiagonalRectangle(
    width,
    height,
    diagonal,
    extensionHeight,
    transformX = 0,
    transformY = 0,
    scale = 1,
    flipX = false,
    flipY = false
) {
    var w = width / 2;
    var h = height / 2;

    var points = [
        [-w, h],                       // A
        [ w - diagonal, h],            // B
        [ w, h - diagonal],            // C
        [ w, -h],                      // D
        [ w - diagonal, -h],           // E
        [ w - diagonal, -h - extensionHeight], // F
        [-w, -h],                      // G
        [-w, -h - extensionHeight]     // H
    ];

    function transform(point) {
        var x = point[0] * scale;
        var y = point[1] * scale;

        if (flipX) x = -x;
        if (flipY) y = -y;

        x += transformX;
        y += transformY;

        return [x, y];
    }

    points = points.map(transform);

    var A = points[0];
    var B = points[1];
    var C = points[2];
    var D = points[3];
    var E = points[4];
    var F = points[5];
    var G = points[6];
    var H = points[7];

    return [
        // Main rectangle / diagonal section
        A[0], A[1], 0.0,
        B[0], B[1], 0.0,
        C[0], C[1], 0.0,

        A[0], A[1], 0.0,
        C[0], C[1], 0.0,
        D[0], D[1], 0.0,

        A[0], A[1], 0.0,
        D[0], D[1], 0.0,
        G[0], G[1], 0.0,

        // Lower extension
        G[0], G[1], 0.0,
        E[0], E[1], 0.0,
        F[0], F[1], 0.0,

        D[0], D[1], 0.0,
        E[0], E[1], 0.0,
        F[0], F[1], 0.0,

        G[0], G[1], 0.0,
        F[0], F[1], 0.0,
        H[0], H[1], 0.0
    ];
}

/**
 * 
 * @param {number} width 
 * @param {number} height 
 * @param {number} diagonal 
 * @param {number} transformX 
 * @param {number} transformY 
 * @param {number} scale 
 * @param {boolean} flipX 
 * @param {boolean} flipY 
 * @returns {Array<number>}
 */
function createShape(
    width,
    height,
    diagonal,
    transformX = 0,
    transformY = 0,
    scale = 1,
    flipX = false,
    flipY = false
) {
    var w = width / 2;
    var h = height / 2;

    var points = [
        [-w, h],                    // A
        [w - diagonal, h],          // B
        [w, h - diagonal],          // C
        [w, -h],                    // D
        [-w, -h]                    // E
    ];

    function transform(point) {
        var x = point[0] * scale;
        var y = point[1] * scale;

        if (flipX) x = -x;
        if (flipY) y = -y;

        x += transformX;
        y += transformY;

        return [x, y];
    }

    points = points.map(transform);

    var A = points[0];
    var B = points[1];
    var C = points[2];
    var D = points[3];
    var E = points[4];

    return [
        // Triangle 1
        A[0], A[1], 0.0,
        B[0], B[1], 0.0,
        C[0], C[1], 0.0,

        // Triangle 2
        A[0], A[1], 0.0,
        C[0], C[1], 0.0,
        D[0], D[1], 0.0,

        // Triangle 3
        A[0], A[1], 0.0,
        D[0], D[1], 0.0,
        E[0], E[1], 0.0
    ];
}

/**
 * Combines multiple vertex arrays into one.
 *
 * @param {...Array<number>} shapes
 * @returns {Array<number>}
 */
function combineShapes(...shapes) {
    return shapes.flat();
}

/**
 * @param {Array<number>} vertices
 * @param {number} transformX
 * @returns {Array<number>}
 */
function transformX(vertices, transformX) {
    return vertices.map((value, index) => {
        if (index % 3 === 0) {
            return value + transformX;
        }

        return value;
    });
}

/**
 * Creates a regular rectangle.
 *
 * @param {number} width
 * @param {number} height
 * @param {number} transformX
 * @param {number} transformY
 * @param {number} scale
 * @param {boolean} flipX
 * @param {boolean} flipY
 * @returns {Array<number>}
 */
function createRectangle(
    width,
    height,
    transformX = 0,
    transformY = 0,
    scale = 1,
    flipX = false,
    flipY = false
) {
    var w = width / 2;
    var h = height / 2;

    var points = [
        [-w,  h], // A
        [ w,  h], // B
        [ w, -h], // C
        [-w, -h]  // D
    ];

    function transform(point) {
        var x = point[0] * scale;
        var y = point[1] * scale;

        if (flipX) {
            x = -x;
        }

        if (flipY) {
            y = -y;
        }

        x += transformX;
        y += transformY;

        return [x, y];
    }

    points = points.map(transform);

    var A = points[0];
    var B = points[1];
    var C = points[2];
    var D = points[3];

    return [
        // Triangle 1
        A[0], A[1], 0.0,
        B[0], B[1], 0.0,
        C[0], C[1], 0.0,

        // Triangle 2
        A[0], A[1], 0.0,
        C[0], C[1], 0.0,
        D[0], D[1], 0.0
    ];
}