
$(function() {

    $(document).on('resetForm', toggleCoachRequiredInputs.bind(null, true))

    $('[type="reset"]').on('click', toggleCoachRequiredInputs.bind(null, true))

    function focusOnTeamName() {
        debugger;
        $('#TeamName').focus()
    }

    // Remove extra fields which might have been added.
    function removeAdditionalCoachFields() {
        Array($('input[name="FirstName"]').length - 1).fill(1).forEach(x => {
            $('#coach-form').remove()
        })
    }

    // Disable coach info when user is the coach.
    $('#i-am-the-coach').on('click', (e) => {
        toggleCoachRequiredInputs(!e.currentTarget.checked, e.currentTarget)
    })

    function toggleCoachRequiredInputs(required) {
        removeAdditionalCoachFields()
        $('#add-another-coach-button').attr('disabled', !required)
        $('#coach-form').find('input')
            .each((_, x) => {
                $(x).attr('disabled', !required).val(null)
            })
        $('[autofocus]').focus()
    }
})


