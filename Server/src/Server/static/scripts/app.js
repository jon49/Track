
(function iffe() {
    'use strict'

    var toArray = x => Array.prototype.slice.call(x),
        selectAll = x => document.querySelectorAll(x),
        header = {
            target: function (xhr) { return xhr.getResponseHeader('X-Target') },
            action: function (xhr) { return xhr.getResponseHeader('X-Action') },
        },

        // Constants
        attrDisabled = 'disabled',
        classLoading = 'loading',

        postSubmitActions = {
            Append: {
                action: function ($el, $target) {
                    $target.append($el)
                }
            },
            Replace: {
                action: function ($el, $target) {
                    $target.parentNode.replaceChild($el, $target)
                }
            },
            ReplaceIn: {
                action: function ($el, $target) {
                    $target.replaceChild($el, $target)
                }
            },
            Prepend: {
                action: function ($el, $target) {
                    $target.prepend($el)
                }
            }
        },

        // Load HTML
        cache = {},
        load = function (xhr, $el, options) {
            options = options || {}
            if (options.cacheResult) {
                cache[xhr.url] = xhr
                return
            }
            if (xhr.status !== 200 && xhr.status !== 304) {
                // TODO show error and message
                return
            }

            ; (options.custom && typeof options.custom === 'function' && options.custom())
            var action, target;
            if ((action = header.action(xhr))
                && (target = header.target(xhr))
                && xhr.responseText) {
                var responsePage = document.createElement('buffer')
                responsePage.innerHTML = xhr.responseText

                toArray(responsePage.querySelectorAll(target))
                    .forEach(function ($el) {
                        var $target
                        if ($target = document.getElementById($el.id)) {
                            postSubmitActions[action].action($el, $target)
                            $el.classList.add('loaded')
                        }
                    })
            }

            $el.classList.remove(classLoading)
            scan()
        },

        // Make pages instant
        // TODO add try catch so if error falls back.
        preloadXHR,
        lastTouchTimestamp,
        mouseoverTimer,
        loadCache = function ($a) {
            load(cache[$a.href], $a)
            delete cache[$a.href]
        },
        preload = function ($a) {
            var xhr = new XMLHttpRequest()
            preloadXHR = xhr

            xhr.withCredentials = true
            xhr.onload = load.bind(null, xhr, $a, { cacheResult: true })
            xhr.open('GET', $a.href, true)
            xhr.send()
        },
        cancelPreload = function () {
            preloadXHR && preloadXHR.abort()
            preloadXHR = null
        },
        mouseoutListener = function() {
            if (mouseoverTimer) {
                clearTimeout(mouseoverTimer)
                mouseoverTimer = null
            }
            else {
                cancelPreload()
            }
        },
        startHoverInstant = function ($el) {
            if (performance.now() - lastTouchTimestamp < 1100) {
                return
            }

            $el.addEventListener('mouseout', mouseoutListener, {passive: true})

            mouseoverTimer = setTimeout(function() {
                preload($el.href)
                mouseoverTimer = null
            }, 65)
        },
        startTouchInstant = function ($el) {
            lastTouchTimestamp = performance.now()

            $el.addEventListener('touchcancel', cancelPreload, {passive: true})
            $el.addEventListener('touchend', cancelPreload, {passive: true})

            preload($el.href)
        },

        // Events
        eventList = [
            ['form', 'data-form-registered', 'submit', false, submitForm ],
            ['a.href', 'data-href-registered', 'mouseover', true, startHoverInstant ],
            ['a.href', 'data-href-touch-registered', 'touchstart', true, startTouchInstant],
            ['a.href', 'data-href-clicked-registered', 'click', false, loadCache]
        ]

        function submitForm($form) {
            var $submit = $form.querySelector('[type=submit]'),
                xhr = new XMLHttpRequest(),
                baseUrl = $form.action,
                acceptField = function (field) {
                    return !!field.name
                    && (field.type !== 'checkbox' && field.type !== 'radio' || field.checked)
                    && (field.tagName !== 'BUTTON' || field === $submit)
                },
                queryString =
                    toArray($form.elements)
                        .filter(acceptField)
                        .map(function (el) { return el.name + '=' + encodeURIComponent(el.value).replace(/%20/g, '+') })
                        .join('&'),
                getUrl = function() {
                    var $url = document.createElement('a')
                    $url.href = (url === '' ? document.URL : url) // IE bug workaround
                    $url.search = queryString
                    return $url.href
                }

            $submit.setAttribute(attrDisabled, '')
            $submit.classList.add(classLoading)

            xhr.withCredentials = true
            xhr.onload = load.bind(null, xhr, $submit, {
                custom: function () {
                    xhr.getResponseHeader('X-Reset-Form') && $form.reset()
                    !xhr.getResponseHeader('X-Disable-Submit') && $submit.removeAttribute(attrDisabled)
                }
            })
            xhr.open($form.method, getUrl(), true)
            xhr.send()
        }

        function scan() {
            eventList.forEach(function (xs) {
                var query = xs[0],
                    registered = xs[1],
                    event = xs[2],
                    passive = xs[3],
                    listener = xs[4]

                toArray(selectAll(query+':not(['+registered+'])'))
                    .forEach(el => {
                        el.addEventListener(event,
                            function (e)
                            {
                                if (!passive) {
                                    e.preventDefault()
                                }
                                listener(e.target)
                            }, { passive })
                        el.setAttribute(registered, '')
                    })
            })
        }

        scan()

}());

