$(".tab-wizard").steps({
    headerTag: "h6"
    , bodyTag: "section"
    , transitionEffect: "fade"
    , titleTemplate: '<span class="step">#index#</span> #title#'
    , onFinished: function (event, currentIndex) {
        swal("Form Submitted!", "Jou nieuwe grafiek is aangemaakt. Je kan hem bekijken op jou dashboard!");
        //soort grafiek
        var checkedValue = $('input[name=TypeGrafiekRadio]:checked').val();
        var TypeGrafiek = $('#TypeGrafiek').val(checkedValue);

        //geselecteerde entiteit
        var selectedvalues = $('select#optgroup').val();
        for (i = 0; i < selectedvalues.length; i++) {
            $('<input>').attr({
                type: 'hidden',
                name: 'EntiteitIds[' + i + ']',
                value: selectedvalues[i]
            }).appendTo('#grafiekForm');
        }

        //geselecteerde cijfer gegevens
        var cijferSelected = $('select#pre-selected-options').val();
        for (i = 0; i < cijferSelected.length; i++) {
            $('<input>').attr({
                type: 'hidden',
                name: 'CijferOpties[' + i + ']',
                value: cijferSelected[i]
            }).appendTo('#grafiekForm');
        }

        var SoortGrafiek = $('select[name=SoortGrafiek]').val()
        $('<input>').attr({
            type: 'hidden',
            name: 'GrafiekSoort',
            value: SoortGrafiek
        }).appendTo('#grafiekForm');

        var naam = $("#Naam").val()
        $('<input>').attr({
            type: 'hidden',
            name: 'Naam',
            value: naam
        }).appendTo('#grafiekForm');

        $.ajax({
            url: '/account/createGrafiek',
            type: 'POST',
            data: $('#grafiekForm').serialize(),
            success: function () {
                wait(3500);
                window.location.href = "/manage";
            },
        });
            
    }
});

function wait(ms) {
    var start = new Date().getTime();
    var end = start;
    while (end < start + ms) {
        end = new Date().getTime();
    }
}

var form = $(".validation-wizard").show();

$(".validation-wizard").steps({
    headerTag: "h6"
    , bodyTag: "section"
    , transitionEffect: "fade"
    , titleTemplate: '<span class="step">#index#</span> #title#'
    , labels: {
        finish: "Submit"
    }
    , onStepChanging: function (event, currentIndex, newIndex) {
        return currentIndex > newIndex || !(3 === newIndex && Number($("#age-2").val()) < 18) && (currentIndex < newIndex && (form.find(".body:eq(" + newIndex + ") label.error").remove(), form.find(".body:eq(" + newIndex + ") .error").removeClass("error")), form.validate().settings.ignore = ":disabled,:hidden", form.valid())
    }
    , onFinishing: function (event, currentIndex) {
        return form.validate().settings.ignore = ":disabled", form.valid()
    }
    , onFinished: function (event, currentIndex) {
         swal("Form Submitted!", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed lorem erat eleifend ex semper, lobortis purus sed.");
    }
}), $(".validation-wizard").validate({
    ignore: "input[type=hidden]"
    , errorClass: "text-danger"
    , successClass: "text-success"
    , highlight: function (element, errorClass) {
        $(element).removeClass(errorClass)
    }
    , unhighlight: function (element, errorClass) {
        $(element).removeClass(errorClass)
    }
    , errorPlacement: function (error, element) {
        error.insertAfter(element)
    }
    , rules: {
        email: {
            email: !0
        }
    }
})