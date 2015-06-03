function andr(name) {
    var requiredCheckboxes = $(':checkbox[required][name = ' + name + ']');
    requiredCheckboxes.change(function () {
        if (requiredCheckboxes.is(':checked')) {
            requiredCheckboxes.removeAttr('required');
            document.getElementById(name).setCustomValidity('');
        } else {
            requiredCheckboxes.attr('required', 'required');
            document.getElementById(name).setCustomValidity('Выбирите варианты ответа');
        }
    });
}




