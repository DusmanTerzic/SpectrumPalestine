(function ($) {
    $.widget("suwala.doubleScroll", {
        options: {
            contentElement: undefined,
            topScrollBarMarkup: '<div class="suwala-doubleScroll-scroll-wrapper" style="height: 20px;"><div class="suwala-doubleScroll-scroll" style="height: 20px"></div></div>',
            topScrollBarInnerSelector: '.suwala-doubleScroll-scroll',
            scrollCss: {
                'overflow-x': 'scroll',
                'overflow-y': 'hidden'
            },
            contentCss: {
                'overflow-x': 'scroll',
                'overflow-y': 'hidden'
            }
        },
        _create: function () {
            var self = this;
            var contentElement;
            var topScrollBar = $($(self.options.topScrollBarMarkup));
            self.element.before(topScrollBar);
            if (self.options.contentElement !== undefined && self.element.find(self.options.contentElement).length !== 0) {
                contentElement = self.element.find(self.options.contentElement);
            } else {
                contentElement = self.element.find('>:first-child');
            }
            topScrollBar.scroll(function () {
                self.element.scrollLeft(topScrollBar.scrollLeft());
            });
            self.element.scroll(function () {
                topScrollBar.scrollLeft(self.element.scrollLeft());
            });
            topScrollBar.css(self.options.scrollCss);
            self.element.css(self.options.contentCss);
            $(self.options.topScrollBarInnerSelector, topScrollBar).width(contentElement.outerWidth());
            topScrollBar.width(self.element.width());
        }
    });
})(jQuery);