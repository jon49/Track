
(function iffe() {
    'use strict'

    var toArray = x => Array.prototype.slice.call(x),
        selectAll = x => document.querySelectorAll(x),

        // Constants
        attrDisabled = 'disabled',
        classLoading = 'loading',

        postSubmitActions = {
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

        toggleClass = $el => {
            var [toggle, target] = $el.getAttribute('data-toggle-class').split(',')
            toArray(selectAll(target))
                .forEach($target => {
                    $target.classList.toggle(toggle)
                })
        },
        toggleAttribute = $el => {
            var [attribute, target] = $el.getAttribute('data-toggle-attribute').split(',')
            toArray(selectAll(target))
                .forEach($target => {
                    if ($target.hasAttribute(attribute)) {
                        $target.setAttribute(attribute)
                    } else {
                        $target.removeAttribute(attribute)
                    }
                })
        }

        // Submit form
        ; ({
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
                                if ($target = document.getElementById($el.id)) {
                                    postSubmitActions[action].action($el, $target)
                                    $el.classList.add('loaded')
                                }
                            })
                    }

                    $submit.classList.remove(classLoading)
                    this.scan()
                }
                xhr.open($form.method, getUrl(), true)
                xhr.send()
            },
            scan = () => {
                [
                    ['form', 'data-form-registered', 'submit', e => {
                        e.preventDefault()
                        submitForm(e.target)
                    }],
                    ['[data-toggle-class]', 'data-toggle-class-registered', 'click', e => toggleClass(e.target)],
                    ['[data-toggle-attribute]', 'data-toggle-attribute-registered', 'click', e => toggleAttribute(e.target)]
                ].forEach(([query, registered, event, listener]) => {
                    toArray(selectAll(`${query}:not([${registered}])`))
                        .forEach(el => {
                            el.addEventListener(event, listener)
                            el.setAttribute(registered, '')
                        })
                })
            }
        }).scan()

}());

