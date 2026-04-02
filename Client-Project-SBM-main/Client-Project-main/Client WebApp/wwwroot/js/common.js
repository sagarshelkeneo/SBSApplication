function resetForm(formId) {
    const form = $(formId);

    if (form.length === 0) return;

    form[0].reset();

    if (form.data('validator')) {
        form.validate().resetForm();
    }

    form.find('.is-invalid').removeClass('is-invalid');
    form.find('select').val('');
}