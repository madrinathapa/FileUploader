$(function myfunction() {

    $(".ImageSize").hover(function () {
        srcAttribute = $(this).attr("src");
        var srcImage = srcAttribute.split('/').pop();
        var changedSrcAttribute = "Image/Hover" + srcImage;
        $(this).attr("src", changedSrcAttribute);
    },
    function () {
        $(this).attr("src", srcAttribute);
    });
});