var freeze = false;

function onMouseClick(event) {
    freeze = !freeze;
}

document.addEventListener('click', onMouseClick, false);

document.addEventListener('keydown', (e) => {
    if(e.key == '32') {
        freeze = true;
    }
}, false);

document.addEventListener('keyup', (e) => {
    if(e.key == '32') {
        freeze = false;
    }
}, false);
