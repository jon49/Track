
(function iffe() {
    'use strict'

    var toArray = x => Array.prototype.slice.call(x),
        getById = x => document.getElementById(x),

        // Constants
        attrDisabled = 'disabled',
        classLoading = 'loading',

        postActions = {
            Append: {
                action: ($el, $target) => {
                    $target.append($el)
                }
            },
            Replace: {
                action: ($el, $target) => {
                    $target.parentNode.replaceChild($el, $target)
                }
            },
            ReplaceIn: {
                action: ($el, $target) => {
                    $target.replaceChild($el, $target)
                }
            },
            Prepend: {
                action: ($el, $target) => {
                    $target.prepend($el)
                }
            }
        },

        // Submit form
        submitForm = $form => {
            var $submit = $form.querySelector('[type=submit]'),
                xhr = new XMLHttpRequest(),
                baseUrl = $form.action,
                acceptField = field =>
                    !!field.name
                    && (field.type !== 'checkbox' && field.type !== 'radio' || field.checked)
                    && (field.tagName !== 'BUTTON' || field === $submit),
                queryString =
                    toArray($form.elements)
                        .filter(acceptField)
                        .map(el => el.name + '=' + encodeURIComponent(el.value).replace(/%20/g, '+'))
                        .join('&'),
                getUrl = () => {
                    var $url = document.createElement('a')
                    $url.href = (url === '' ? document.URL : url) // IE bug workaround
                    $url.search = queryString
                    return $url.href
                }

            $submit.setAttribute(attrDisabled, '')
            $submit.classList.add(classLoading)

            xhr.withCredentials = true
            xhr.onload = function processResponse() {
                if (xhr.status !== 200 && xhr.status !== 304) {
                    // TODO show error and message
                    return
                }

                if (xhr.getResponseHeader('X-Reset-Form')) {
                    $form.reset()
                }
                if (!xhr.getResponseHeader('X-Disable-Submit')) {
                    $submit.removeAttribute(attrDisabled)
                }
                var action, target;
                if ((action = xhr.getResponseHeader('X-Action'))
                 && (target = xhr.getResponseHeader('X-Target'))
                 && xhr.responseText) {
                    var responsePage = document.createElement('buffer')
                    responsePage.innerHTML = xhr.responseText

                    toArray(responsePage.querySelectorAll(target))
                        .forEach($el => {
                            var $target
                            if ($target = getById($el.id)) {
                                postActions[action].action($el, $target)
                                $el.classList.add('loaded')
                            }
                        })
                }

                $submit.classList.remove(classLoading)
            }
            xhr.open($form.method, getUrl(), true)
            xhr.send()
        },
        yep = () => submitForm()

}());

