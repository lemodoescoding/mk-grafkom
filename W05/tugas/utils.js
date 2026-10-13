/**
 * Creates a chamfered rectangle. This is JSDoc for type hinting
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
 * Combines multiple vertex arrays into one. This is JSDoc used for type hinting
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
 * Creates a regular rectangle. This is JSDoc used for type hinting
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
