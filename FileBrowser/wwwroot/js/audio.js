(function () {
    var main = document.querySelector('.audio-warp');
    if (main) {
        main.style.opacity = 1;
    }

    // player
    var player = document.querySelector('audio');
    var source = player.querySelector('source');
    source.src = audioUrl;
    player.onloadeddata = function () {
        var volume = storage.volume();
        player.volume = volume;
    };
    player.onvolumechange = function () {
        var volume = player.volume;
        storage.setVolume(volume);
    };
    player.load();
    player.pause();
})();
