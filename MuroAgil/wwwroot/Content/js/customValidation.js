$(document).ready(function () {
    $.validator.setDefaults({
        errorElement: "span",
        highlight: function (element, errorClass, validClass) {
            var parent = $(element).parent(".form-group");
            $(parent).find("input").addClass("is-invalid").removeClass("is-valid");
            $(parent).find("select").addClass("is-invalid").removeClass("is-valid");
            $(parent).find("textarea").addClass("is-invalid").removeClass("is-valid");
            $(parent).find("label").addClass("text-danger").removeClass("text-success");
            $(parent).find("span").addClass("text-danger").removeClass("text-success");
        },
        unhighlight: function (element, errorClass, validClass) {
            var parent = $(element).parent(".form-group");
            $(parent).find("input")/*.addClass("is-valid")*/.removeClass("is-invalid");
            $(parent).find("select")/*.addClass("is-valid")*/.removeClass("is-invalid");
            $(parent).find("textarea")/*.addClass("is-valid")*/.removeClass("is-invalid");
            $(parent).find("label")/*.addClass("text-success")*/.removeClass("text-danger");
            $(parent).find("span")/*.addClass("text-success")*/.removeClass("text-danger");
        }
    });
});