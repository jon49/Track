// Behavior.js https://github.com/QuickenLoans/Behaviors.js
(function spliceIntoIIFE() {
    'use strict'

    var toArray = x => Array.prototype.slice.call(x),
        pipe = (...fs) => x => fs.reduce((acc, f) => f(acc), x),
        querySelectorAll = query => toArray(document.querySelectorAll(query)),
        registered = 'data-track-registered',
        events = {
            'data-track-replace': {
                action: (e) => 'yep',
            },
            'data-track-append': {
                action: (e) => 'nope',
            }
        },
        acceptField = field =>
            !!field.name
            && (field.type !== 'checkbox' && field.type !== 'radio' || field.checked)
            && (field.tagName !== 'BUTTON' || field === this.submittingButton),
        serializeForm = form =>
            toArray(form.elements)
                .filter(acceptField)
                .map(element => element.name + '=' + encodeURIComponent(element.value).replace(/%20/g, '+') )
                .join('&'),
        composeURL = (url, queryString) => {
            var urlElement = document.createElement('a')
            urlElement.href = (url === '' ? document.URL : url) //the conditional is a workaround for IE bug
            urlElement.search = queryString
            return urlElement.href
        },
        getTargetElements = element => {
            var selector = element.getAttribute(this.name)
            return toArray(document.querySelectorAll(selector))
        },
        formHandler = e => {
            e.preventDefault()
            var form = event.target,
                queryString = serializeForm(form),
                url = composeURL(form.action, queryString)//,
            //    targets = getTargetElements(form)

            //this.splice(form.method, url, targets)
        },
        eventNames = Object.keys(events).map(key => events[key].name),
        scan = () => {
            eventNames
            .forEach(data => {
                toArray(querySelectorAll('['+data+']:not(['+registered+'])'))
                .forEach(x => {
                    if (x.tagName === 'FORM') {
                        x.addEventListener('submit', events[data].action)
                    } else {
                        x.addEventListener('click', events[data].action)
                    }
                    x.setAttribute(registered, '')
                })
            })
        }
        ;



    ({

        scan: function scan() {
            toArray(querySelectorAll('['++']'))
                .forEach(this.trackForm, this)
            toArray(querySelectorAll('a' + this.selector))
                .forEach(this.trackLink, this)
        },

        trackForm: function trackForm(form) {
            this.trackLastClickedButton(form)
            form.addEventListener('submit', this.processFormSubmit.bind(this))
            form.setAttribute(this.registered, '')
        },

        processFormSubmit: function processFormSubmit(event) {
            if (!!this.submittingButton && this.submittingButton.hasAttribute(this.disabledMarker)) {
                return //normal form submit with no AJAX
            }
            event.preventDefault()

            var form = event.target,
                queryString = this.serializeForm(form),
                url = this.composeURL(form.action, queryString),
                targets = this.getTargetElements(form)

            this.splice(form.method, url, targets)
        },

        captureSubmitter: function captureSubmitter(event) {
            this.submittingButton = event.target
        },

        trackLastClickedButton: function trackLastClickedButton(form) { //because clicked button's value gets added to query
            function addButtonListener(button) {
                button.addEventListener('click', this.captureSubmitter.bind(this))
            }

            toArray(form.querySelectorAll('button[type=submit]'))
                .forEach(addButtonListener, this)
        },

        trackLink: function trackLink(link) {
            link.addEventListener('click', this.processLinkClick.bind(this))
            link.setAttribute(this.registered, '')
        },

        processLinkClick: function processLinkClick(event) {
            event.preventDefault()

            var link = event.target,
                targets = this.getTargetElements(link)
            this.splice('GET', link.href, targets)
        },

        pageLoadHandler: function pageLoadHandler(targets, responseText) {
            var nextPage = this.parsePage(responseText)
            targets.forEach(this.replaceWithTwin.bind(this, nextPage))
        },


        splice: function splice(method, url, targets) {
            var processPage = this.pageLoadHandler.bind(this, targets)

            targets.forEach(function setLoading(target) {
                target.setAttribute(this.loading, true)
            }, this)

            this.loadPage(method, url, processPage)
        },




        replaceWithTwin: function replaceWithTwin(pageWithReplacement, targetElement) {
            var twinElement = pageWithReplacement.querySelector('#' + targetElement.id)
            targetElement.removeAttribute(this.loading) //for CSS spinners

            if (!twinElement) {
                return
            }

            twinElement.setAttribute(this.loaded, '') //for CSS fade-in effects
            targetElement.parentNode.replaceChild(twinElement, targetElement) //also would work, but probably less efficient: targetElement.outerHTML = twinElement.outerHTML
        },

        parsePage: function parsePage(htmlString) {
            var responsePage = document.createElement('buffer') //this will create a new document, the name does not matter
            responsePage.innerHTML = htmlString
            return responsePage
        },

        loadPage: function loadPage(method, url, finished) {
            var request = new XMLHttpRequest()
            request.onload = function processResponse() {
                if (request.status !== 200 && request.status !== 304) {
                    return
                }
                finished(request.responseText)
            }
            request.open(method, url, true)
            request.send()
        }
    }).init()
}())


