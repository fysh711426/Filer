(function () {
    var main = document.querySelector('.video-warp');
    if (main) {
        main.style.opacity = 1;
    }

    // player
    var player = videojs('player', {
        sources: [{ src: videoUrl }],
        controls: true,
        autoplay: false,
        preload: 'auto',
        fluid: true,
        controlBar: {
            pictureInPictureToggle: false
        }
    });
    player.on('ready', function () {
        var volume = storage.volume();
        player.volume(volume);
    });
    player.on('volumechange', function () {
        var volume = player.volume();
        storage.setVolume(volume);
    });
})();
