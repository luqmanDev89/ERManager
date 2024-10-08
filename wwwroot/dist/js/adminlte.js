/*!
 * AdminLTE v3.0.0-rc.1 (https://adminlte.io)
 * Copyright 2014-2024 Colorlib <http://colorlib.com>
 * Licensed under MIT (https://github.com/almasaeed2010/AdminLTE/blob/master/LICENSE)
 */
(function (global, factory) {
  typeof exports === 'object' && typeof module !== 'undefined' ? factory(exports) :
  typeof define === 'function' && define.amd ? define(['exports'], factory) :
  (global = global || self, factory(global.adminlte = {}));
}(this, (function (exports) { 'use strict';

  /**
   * --------------------------------------------
   * AdminLTE ControlSidebar.js
   * License MIT
   * --------------------------------------------
   */

  const ControlSidebar = ($ => {
    /**
     * Constants
     * ====================================================
     */

    const NAME = 'ControlSidebar';
    const DATA_KEY = 'lte.controlsidebar';
    const EVENT_KEY = `.${DATA_KEY}`;
    const JQUERY_NO_CONFLICT = $.fn[NAME];
    const Event = {
      COLLAPSED: `collapsed${EVENT_KEY}`,
      EXPANDED: `expanded${EVENT_KEY}`
    };
    const Selector = {
      CONTROL_SIDEBAR: '.control-sidebar',
      CONTROL_SIDEBAR_CONTENT: '.control-sidebar-content',
      DATA_TOGGLE: '[data-widget="control-sidebar"]',
      CONTENT: '.content-wrapper',
      HEADER: '.main-header',
      FOOTER: '.main-footer'
    };
    const ClassName = {
      CONTROL_SIDEBAR_ANIMATE: 'control-sidebar-animate',
      CONTROL_SIDEBAR_OPEN: 'control-sidebar-open',
      CONTROL_SIDEBAR_SLIDE: 'control-sidebar-slide-open',
      LAYOUT_FIXED: 'layout-fixed',
      NAVBAR_FIXED: 'layout-navbar-fixed',
      NAVBAR_SM_FIXED: 'layout-sm-navbar-fixed',
      NAVBAR_MD_FIXED: 'layout-md-navbar-fixed',
      NAVBAR_LG_FIXED: 'layout-lg-navbar-fixed',
      NAVBAR_XL_FIXED: 'layout-xl-navbar-fixed',
      FOOTER_FIXED: 'layout-footer-fixed',
      FOOTER_SM_FIXED: 'layout-sm-footer-fixed',
      FOOTER_MD_FIXED: 'layout-md-footer-fixed',
      FOOTER_LG_FIXED: 'layout-lg-footer-fixed',
      FOOTER_XL_FIXED: 'layout-xl-footer-fixed'
    };

    /**
     * Class Definition
     * ====================================================
     */

    class ControlSidebar {
      constructor(element, config) {
        this._element = element;
        this._config = config;
        this._init();
      }

      // Public

      show() {
        // Show the control sidebar
        if (this._config.controlsidebarSlide) {
          $('html').addClass(ClassName.CONTROL_SIDEBAR_ANIMATE);
          $('body').removeClass(ClassName.CONTROL_SIDEBAR_SLIDE).delay(300).queue(function () {
            $(Selector.CONTROL_SIDEBAR).hide();
            $('html').removeClass(ClassName.CONTROL_SIDEBAR_ANIMATE);
            $(this).dequeue();
          });
        } else {
          $('body').removeClass(ClassName.CONTROL_SIDEBAR_OPEN);
        }
        const expandedEvent = $.Event(Event.EXPANDED);
        $(this._element).trigger(expandedEvent);
      }
      collapse() {
        // Collapse the control sidebar
        if (this._config.controlsidebarSlide) {
          $('html').addClass(ClassName.CONTROL_SIDEBAR_ANIMATE);
          $(Selector.CONTROL_SIDEBAR).show().delay(10).queue(function () {
            $('body').addClass(ClassName.CONTROL_SIDEBAR_SLIDE).delay(300).queue(function () {
              $('html').removeClass(ClassName.CONTROL_SIDEBAR_ANIMATE);
              $(this).dequeue();
            });
            $(this).dequeue();
          });
        } else {
          $('body').addClass(ClassName.CONTROL_SIDEBAR_OPEN);
        }
        const collapsedEvent = $.Event(Event.COLLAPSED);
        $(this._element).trigger(collapsedEvent);
      }
      toggle() {
        const shouldOpen = $('body').hasClass(ClassName.CONTROL_SIDEBAR_OPEN) || $('body').hasClass(ClassName.CONTROL_SIDEBAR_SLIDE);
        if (shouldOpen) {
          // Open the control sidebar
          this.show();
        } else {
          // Close the control sidebar
          this.collapse();
        }
      }

      // Private

      _init() {
        this._fixHeight();
        this._fixScrollHeight();
        $(window).resize(() => {
          this._fixHeight();
          this._fixScrollHeight();
        });
        $(window).scroll(() => {
          if ($('body').hasClass(ClassName.CONTROL_SIDEBAR_OPEN) || $('body').hasClass(ClassName.CONTROL_SIDEBAR_SLIDE)) {
            this._fixScrollHeight();
          }
        });
      }
      _fixScrollHeight() {
        const heights = {
          scroll: $(document).height(),
          window: $(window).height(),
          header: $(Selector.HEADER).outerHeight(),
          footer: $(Selector.FOOTER).outerHeight()
        };
        const positions = {
          bottom: Math.abs(heights.window + $(window).scrollTop() - heights.scroll),
          top: $(window).scrollTop()
        };
        let navbarFixed = false;
        let footerFixed = false;
        if ($('body').hasClass(ClassName.LAYOUT_FIXED)) {
          if ($('body').hasClass(ClassName.NAVBAR_FIXED) || $('body').hasClass(ClassName.NAVBAR_SM_FIXED) || $('body').hasClass(ClassName.NAVBAR_MD_FIXED) || $('body').hasClass(ClassName.NAVBAR_LG_FIXED) || $('body').hasClass(ClassName.NAVBAR_XL_FIXED)) {
            if ($(Selector.HEADER).css("position") === "fixed") {
              navbarFixed = true;
            }
          }
          if ($('body').hasClass(ClassName.FOOTER_FIXED) || $('body').hasClass(ClassName.FOOTER_SM_FIXED) || $('body').hasClass(ClassName.FOOTER_MD_FIXED) || $('body').hasClass(ClassName.FOOTER_LG_FIXED) || $('body').hasClass(ClassName.FOOTER_XL_FIXED)) {
            if ($(Selector.FOOTER).css("position") === "fixed") {
              footerFixed = true;
            }
          }
          if (positions.top === 0 && positions.bottom === 0) {
            $(Selector.CONTROL_SIDEBAR).css('bottom', heights.footer);
            $(Selector.CONTROL_SIDEBAR).css('top', heights.header);
            $(Selector.CONTROL_SIDEBAR + ', ' + Selector.CONTROL_SIDEBAR + ' ' + Selector.CONTROL_SIDEBAR_CONTENT).css('height', heights.window - (heights.header + heights.footer));
          } else if (positions.bottom <= heights.footer) {
            if (footerFixed === false) {
              $(Selector.CONTROL_SIDEBAR).css('bottom', heights.footer - positions.bottom);
              $(Selector.CONTROL_SIDEBAR + ', ' + Selector.CONTROL_SIDEBAR + ' ' + Selector.CONTROL_SIDEBAR_CONTENT).css('height', heights.window - (heights.footer - positions.bottom));
            } else {
              $(Selector.CONTROL_SIDEBAR).css('bottom', heights.footer);
            }
          } else if (positions.top <= heights.header) {
            if (navbarFixed === false) {
              $(Selector.CONTROL_SIDEBAR).css('top', heights.header - positions.top);
              $(Selector.CONTROL_SIDEBAR + ', ' + Selector.CONTROL_SIDEBAR + ' ' + Selector.CONTROL_SIDEBAR_CONTENT).css('height', heights.window - (heights.header - positions.top));
            } else {
              $(Selector.CONTROL_SIDEBAR).css('top', heights.header);
            }
          } else {
            if (navbarFixed === false) {
              $(Selector.CONTROL_SIDEBAR).css('top', 0);
              $(Selector.CONTROL_SIDEBAR + ', ' + Selector.CONTROL_SIDEBAR + ' ' + Selector.CONTROL_SIDEBAR_CONTENT).css('height', heights.window);
            } else {
              $(Selector.CONTROL_SIDEBAR).css('top', heights.header);
            }
          }
        }
      }
      _fixHeight() {
        const heights = {
          window: $(window).height(),
          header: $(Selector.HEADER).outerHeight(),
          footer: $(Selector.FOOTER).outerHeight()
        };
        if ($('body').hasClass(ClassName.LAYOUT_FIXED)) {
          let sidebarHeight = heights.window - heights.header;
          if ($('body').hasClass(ClassName.FOOTER_FIXED) || $('body').hasClass(ClassName.FOOTER_SM_FIXED) || $('body').hasClass(ClassName.FOOTER_MD_FIXED) || $('body').hasClass(ClassName.FOOTER_LG_FIXED) || $('body').hasClass(ClassName.FOOTER_XL_FIXED)) {
            if ($(Selector.FOOTER).css("position") === "fixed") {
              sidebarHeight = heights.window - heights.header - heights.footer;
            }
          }
          $(Selector.CONTROL_SIDEBAR + ' ' + Selector.CONTROL_SIDEBAR_CONTENT).css('height', sidebarHeight);
          if (typeof $.fn.overlayScrollbars !== 'undefined') {
            $(Selector.CONTROL_SIDEBAR + ' ' + Selector.CONTROL_SIDEBAR_CONTENT).overlayScrollbars({
              className: this._config.scrollbarTheme,
              sizeAutoCapable: true,
              scrollbars: {
                autoHide: this._config.scrollbarAutoHide,
                clickScrolling: true
              }
            });
          }
        }
      }

      // Static

      static _jQueryInterface(operation) {
        return this.each(function () {
          let data = $(this).data(DATA_KEY);
          if (!data) {
            data = new ControlSidebar(this, $(this).data());
            $(this).data(DATA_KEY, data);
          }
          if (data[operation] === 'undefined') {
            throw new Error(`${operation} is not a function`);
          }
          data[operation]();
        });
      }
    }

    /**
     *
     * Data Api implementation
     * ====================================================
     */
    $(document).on('click', Selector.DATA_TOGGLE, function (event) {
      event.preventDefault();
      ControlSidebar._jQueryInterface.call($(this), 'toggle');
    });

    /**
     * jQuery API
     * ====================================================
     */

    $.fn[NAME] = ControlSidebar._jQueryInterface;
    $.fn[NAME].Constructor = ControlSidebar;
    $.fn[NAME].noConflict = function () {
      $.fn[NAME] = JQUERY_NO_CONFLICT;
      return ControlSidebar._jQueryInterface;
    };
    return ControlSidebar;
  })(jQuery);

  /**
   * --------------------------------------------
   * AdminLTE Layout.js
   * License MIT
   * --------------------------------------------
   */

  const Layout = ($ => {
    /**
     * Constants
     * ====================================================
     */

    const NAME = 'Layout';
    const DATA_KEY = 'lte.layout';
    const JQUERY_NO_CONFLICT = $.fn[NAME];
    const Selector = {
      HEADER: '.main-header',
      MAIN_SIDEBAR: '.main-sidebar',
      SIDEBAR: '.main-sidebar .sidebar',
      CONTENT: '.content-wrapper',
      BRAND: '.brand-link',
      CONTENT_HEADER: '.content-header',
      WRAPPER: '.wrapper',
      CONTROL_SIDEBAR: '.control-sidebar',
      LAYOUT_FIXED: '.layout-fixed',
      FOOTER: '.main-footer'
    };
    const ClassName = {
      HOLD: 'hold-transition',
      SIDEBAR: 'main-sidebar',
      CONTENT_FIXED: 'content-fixed',
      SIDEBAR_FOCUSED: 'sidebar-focused',
      LAYOUT_FIXED: 'layout-fixed',
      NAVBAR_FIXED: 'layout-navbar-fixed',
      FOOTER_FIXED: 'layout-footer-fixed'
    };
    const Default = {
      scrollbarTheme: 'os-theme-light',
      scrollbarAutoHide: 'l'
    };

    /**
     * Class Definition
     * ====================================================
     */

    class Layout {
      constructor(element, config) {
        this._config = config;
        this._element = element;
        this._init();
      }

      // Public

      fixLayoutHeight() {
        const heights = {
          window: $(window).height(),
          header: $(Selector.HEADER).outerHeight(),
          footer: $(Selector.FOOTER).outerHeight(),
          sidebar: $(Selector.SIDEBAR).height()
        };
        const max = this._max(heights);
        if ($('body').hasClass(ClassName.LAYOUT_FIXED)) {
          $(Selector.CONTENT).css('min-height', max - heights.header - heights.footer);
          // $(Selector.SIDEBAR).css('min-height', max - heights.header)

          if (typeof $.fn.overlayScrollbars !== 'undefined') {
            $(Selector.SIDEBAR).overlayScrollbars({
              className: this._config.scrollbarTheme,
              sizeAutoCapable: true,
              scrollbars: {
                autoHide: this._config.scrollbarAutoHide,
                clickScrolling: true
              }
            });
          }
        } else {
          if (heights.window > heights.sidebar) {
            $(Selector.CONTENT).css('min-height', heights.window - heights.header - heights.footer);
          } else {
            $(Selector.CONTENT).css('min-height', heights.sidebar - heights.header);
          }
        }
      }

      // Private

      _init() {
        // Enable transitions
        $('body').removeClass(ClassName.HOLD);

        // Activate layout height watcher
        this.fixLayoutHeight();
        $(Selector.SIDEBAR).on('collapsed.lte.treeview expanded.lte.treeview collapsed.lte.pushmenu expanded.lte.pushmenu', () => {
          this.fixLayoutHeight();
        });
        $(window).resize(() => {
          this.fixLayoutHeight();
        });
        $('body, html').css('height', 'auto');
      }
      _max(numbers) {
        // Calculate the maximum number in a list
        let max = 0;
        Object.keys(numbers).forEach(key => {
          if (numbers[key] > max) {
            max = numbers[key];
          }
        });
        return max;
      }

      // Static

      static _jQueryInterface(config) {
        return this.each(function () {
          let data = $(this).data(DATA_KEY);
          const _config = $.extend({}, Default, $(this).data());
          if (!data) {
            data = new Layout($(this), _config);
            $(this).data(DATA_KEY, data);
          }
          if (config === 'init') {
            data[config]();
          }
        });
      }
    }

    /**
     * Data API
     * ====================================================
     */

    $(window).on('load', () => {
      Layout._jQueryInterface.call($('body'));
    });
    $(Selector.SIDEBAR + ' a').on('focusin', () => {
      $(Selector.MAIN_SIDEBAR).addClass(ClassName.SIDEBAR_FOCUSED);
    });
    $(Selector.SIDEBAR + ' a').on('focusout', () => {
      $(Selector.MAIN_SIDEBAR).removeClass(ClassName.SIDEBAR_FOCUSED);
    });

    /**
     * jQuery API
     * ====================================================
     */

    $.fn[NAME] = Layout._jQueryInterface;
    $.fn[NAME].Constructor = Layout;
    $.fn[NAME].noConflict = function () {
      $.fn[NAME] = JQUERY_NO_CONFLICT;
      return Layout._jQueryInterface;
    };
    return Layout;
  })(jQuery);

  /**
   * --------------------------------------------
   * AdminLTE PushMenu.js
   * License MIT
   * --------------------------------------------
   */

  const PushMenu = ($ => {
    /**
     * Constants
     * ====================================================
     */

    const NAME = 'PushMenu';
    const DATA_KEY = 'lte.pushmenu';
    const EVENT_KEY = `.${DATA_KEY}`;
    const JQUERY_NO_CONFLICT = $.fn[NAME];
    const Event = {
      COLLAPSED: `collapsed${EVENT_KEY}`,
      SHOWN: `shown${EVENT_KEY}`
    };
    const Default = {
      autoCollapseSize: false,
      screenCollapseSize: 768,
      enableRemember: false,
      noTransitionAfterReload: true
    };
    const Selector = {
      TOGGLE_BUTTON: '[data-widget="pushmenu"]',
      SIDEBAR_MINI: '.sidebar-mini',
      SIDEBAR_COLLAPSED: '.sidebar-collapse',
      BODY: 'body',
      OVERLAY: '#sidebar-overlay',
      WRAPPER: '.wrapper'
    };
    const ClassName = {
      SIDEBAR_OPEN: 'sidebar-open',
      COLLAPSED: 'sidebar-collapse',
      OPEN: 'sidebar-open',
      SIDEBAR_MINI: 'sidebar-mini'
    };

    /**
     * Class Definition
     * ====================================================
     */

    class PushMenu {
      constructor(element, options) {
        this._element = element;
        this._options = $.extend({}, Default, options);
        this._init();
        if (!$(Selector.OVERLAY).length) {
          this._addOverlay();
        }
      }

      // Public

      show() {
        $(Selector.BODY).addClass(ClassName.OPEN).removeClass(ClassName.COLLAPSED);
        if (this._options.enableRemember) {
          localStorage.setItem(`remember${EVENT_KEY}`, ClassName.OPEN);
        }
        const shownEvent = $.Event(Event.SHOWN);
        $(this._element).trigger(shownEvent);
      }
      collapse() {
        $(Selector.BODY).removeClass(ClassName.OPEN).addClass(ClassName.COLLAPSED);
        if (this._options.enableRemember) {
          localStorage.setItem(`remember${EVENT_KEY}`, ClassName.COLLAPSED);
        }
        const collapsedEvent = $.Event(Event.COLLAPSED);
        $(this._element).trigger(collapsedEvent);
      }
      isShown() {
        if ($(window).width() >= this._options.screenCollapseSize) {
          return !$(Selector.BODY).hasClass(ClassName.COLLAPSED);
        } else {
          return $(Selector.BODY).hasClass(ClassName.OPEN);
        }
      }
      toggle() {
        if (this.isShown()) {
          this.collapse();
        } else {
          this.show();
        }
      }
      autoCollapse() {
        if (this._options.autoCollapseSize) {
          if ($(window).width() <= this._options.autoCollapseSize) {
            if (this.isShown()) {
              this.toggle();
            }
          } else {
            if (!this.isShown()) {
              this.toggle();
            }
          }
        }
      }
      remember() {
        if (this._options.enableRemember) {
          var toggleState = localStorage.getItem(`remember${EVENT_KEY}`);
          if (toggleState == ClassName.COLLAPSED) {
            if (this._options.noTransitionAfterReload) {
              $("body").addClass('hold-transition').addClass(ClassName.COLLAPSED).delay(10).queue(function () {
                $(this).removeClass('hold-transition');
                $(this).dequeue();
              });
            } else {
              $("body").addClass(ClassName.COLLAPSED);
            }
          }
        }
      }

      // Private

      _init() {
        this.remember();
        this.autoCollapse();
        $(window).resize(() => {
          this.autoCollapse();
        });
      }
      _addOverlay() {
        const overlay = $('<div />', {
          id: 'sidebar-overlay'
        });
        overlay.on('click', () => {
          this.collapse();
        });
        $(Selector.WRAPPER).append(overlay);
      }

      // Static

      static _jQueryInterface(operation) {
        return this.each(function () {
          let data = $(this).data(DATA_KEY);
          const _options = $.extend({}, Default, $(this).data());
          if (!data) {
            data = new PushMenu(this, _options);
            $(this).data(DATA_KEY, data);
          }
          if (operation === 'toggle') {
            data[operation]();
          }
        });
      }
    }

    /**
     * Data API
     * ====================================================
     */

    $(document).on('click', Selector.TOGGLE_BUTTON, event => {
      event.preventDefault();
      let button = event.currentTarget;
      if ($(button).data('widget') !== 'pushmenu') {
        button = $(button).closest(Selector.TOGGLE_BUTTON);
      }
      PushMenu._jQueryInterface.call($(button), 'toggle');
    });
    $(window).on('load', () => {
      PushMenu._jQueryInterface.call($(Selector.TOGGLE_BUTTON));
    });

    /**
     * jQuery API
     * ====================================================
     */

    $.fn[NAME] = PushMenu._jQueryInterface;
    $.fn[NAME].Constructor = PushMenu;
    $.fn[NAME].noConflict = function () {
      $.fn[NAME] = JQUERY_NO_CONFLICT;
      return PushMenu._jQueryInterface;
    };
    return PushMenu;
  })(jQuery);

  /**
   * --------------------------------------------
   * AdminLTE Treeview.js
   * License MIT
   * --------------------------------------------
   */

  const Treeview = ($ => {
    /**
     * Constants
     * ====================================================
     */

    const NAME = 'Treeview';
    const DATA_KEY = 'lte.treeview';
    const EVENT_KEY = `.${DATA_KEY}`;
    const JQUERY_NO_CONFLICT = $.fn[NAME];
    const Event = {
      SELECTED: `selected${EVENT_KEY}`,
      EXPANDED: `expanded${EVENT_KEY}`,
      COLLAPSED: `collapsed${EVENT_KEY}`,
      LOAD_DATA_API: `load${EVENT_KEY}`
    };
    const Selector = {
      LI: '.nav-item',
      LINK: '.nav-link',
      TREEVIEW_MENU: '.nav-treeview',
      OPEN: '.menu-open',
      DATA_WIDGET: '[data-widget="treeview"]'
    };
    const ClassName = {
      LI: 'nav-item',
      LINK: 'nav-link',
      TREEVIEW_MENU: 'nav-treeview',
      OPEN: 'menu-open'
    };
    const Default = {
      trigger: `${Selector.DATA_WIDGET} ${Selector.LINK}`,
      animationSpeed: 300,
      accordion: true
    };

    /**
     * Class Definition
     * ====================================================
     */
    class Treeview {
      constructor(element, config) {
        this._config = config;
        this._element = element;
      }

      // Public

      init() {
        this._setupListeners();
      }
      expand(treeviewMenu, parentLi) {
        const expandedEvent = $.Event(Event.EXPANDED);
        if (this._config.accordion) {
          const openMenuLi = parentLi.siblings(Selector.OPEN).first();
          const openTreeview = openMenuLi.find(Selector.TREEVIEW_MENU).first();
          this.collapse(openTreeview, openMenuLi);
        }
        treeviewMenu.stop().slideDown(this._config.animationSpeed, () => {
          parentLi.addClass(ClassName.OPEN);
          $(this._element).trigger(expandedEvent);
        });
      }
      collapse(treeviewMenu, parentLi) {
        const collapsedEvent = $.Event(Event.COLLAPSED);
        treeviewMenu.stop().slideUp(this._config.animationSpeed, () => {
          parentLi.removeClass(ClassName.OPEN);
          $(this._element).trigger(collapsedEvent);
          treeviewMenu.find(`${Selector.OPEN} > ${Selector.TREEVIEW_MENU}`).slideUp();
          treeviewMenu.find(Selector.OPEN).removeClass(ClassName.OPEN);
        });
      }
      toggle(event) {
        const $relativeTarget = $(event.currentTarget);
        const treeviewMenu = $relativeTarget.next();
        if (!treeviewMenu.is(Selector.TREEVIEW_MENU)) {
          return;
        }
        event.preventDefault();
        const parentLi = $relativeTarget.parents(Selector.LI).first();
        const isOpen = parentLi.hasClass(ClassName.OPEN);
        if (isOpen) {
          this.collapse($(treeviewMenu), parentLi);
        } else {
          this.expand($(treeviewMenu), parentLi);
        }
      }

      // Private

      _setupListeners() {
        $(document).on('click', this._config.trigger, event => {
          this.toggle(event);
        });
      }

      // Static

      static _jQueryInterface(config) {
        return this.each(function () {
          let data = $(this).data(DATA_KEY);
          const _config = $.extend({}, Default, $(this).data());
          if (!data) {
            data = new Treeview($(this), _config);
            $(this).data(DATA_KEY, data);
          }
          if (config === 'init') {
            data[config]();
          }
        });
      }
    }

    /**
     * Data API
     * ====================================================
     */

    $(window).on(Event.LOAD_DATA_API, () => {
      $(Selector.DATA_WIDGET).each(function () {
        Treeview._jQueryInterface.call($(this), 'init');
      });
    });

    /**
     * jQuery API
     * ====================================================
     */

    $.fn[NAME] = Treeview._jQueryInterface;
    $.fn[NAME].Constructor = Treeview;
    $.fn[NAME].noConflict = function () {
      $.fn[NAME] = JQUERY_NO_CONFLICT;
      return Treeview._jQueryInterface;
    };
    return Treeview;
  })(jQuery);

  /**
   * --------------------------------------------
   * AdminLTE DirectChat.js
   * License MIT
   * --------------------------------------------
   */

  const DirectChat = ($ => {
    /**
     * Constants
     * ====================================================
     */

    const NAME = 'DirectChat';
    const DATA_KEY = 'lte.directchat';
    const JQUERY_NO_CONFLICT = $.fn[NAME];
    const Event = {
      TOGGLED: `toggled{EVENT_KEY}`
    };
    const Selector = {
      DATA_TOGGLE: '[data-widget="chat-pane-toggle"]',
      DIRECT_CHAT: '.direct-chat'
    };
    const ClassName = {
      DIRECT_CHAT_OPEN: 'direct-chat-contacts-open'
    };

    /**
     * Class Definition
     * ====================================================
     */

    class DirectChat {
      constructor(element, config) {
        this._element = element;
      }
      toggle() {
        $(this._element).parents(Selector.DIRECT_CHAT).first().toggleClass(ClassName.DIRECT_CHAT_OPEN);
        const toggledEvent = $.Event(Event.TOGGLED);
        $(this._element).trigger(toggledEvent);
      }

      // Static

      static _jQueryInterface(config) {
        return this.each(function () {
          let data = $(this).data(DATA_KEY);
          if (!data) {
            data = new DirectChat($(this));
            $(this).data(DATA_KEY, data);
          }
          data[config]();
        });
      }
    }

    /**
     *
     * Data Api implementation
     * ====================================================
     */

    $(document).on('click', Selector.DATA_TOGGLE, function (event) {
      if (event) event.preventDefault();
      DirectChat._jQueryInterface.call($(this), 'toggle');
    });

    /**
     * jQuery API
     * ====================================================
     */

    $.fn[NAME] = DirectChat._jQueryInterface;
    $.fn[NAME].Constructor = DirectChat;
    $.fn[NAME].noConflict = function () {
      $.fn[NAME] = JQUERY_NO_CONFLICT;
      return DirectChat._jQueryInterface;
    };
    return DirectChat;
  })(jQuery);

  /**
   * --------------------------------------------
   * AdminLTE TodoList.js
   * License MIT
   * --------------------------------------------
   */

  const TodoList = ($ => {
    /**
     * Constants
     * ====================================================
     */

    const NAME = 'TodoList';
    const DATA_KEY = 'lte.todolist';
    const JQUERY_NO_CONFLICT = $.fn[NAME];
    const Selector = {
      DATA_TOGGLE: '[data-widget="todo-list"]'
    };
    const ClassName = {
      TODO_LIST_DONE: 'done'
    };
    const Default = {
      onCheck: function (item) {
        return item;
      },
      onUnCheck: function (item) {
        return item;
      }
    };

    /**
     * Class Definition
     * ====================================================
     */

    class TodoList {
      constructor(element, config) {
        this._config = config;
        this._element = element;
        this._init();
      }

      // Public

      toggle(item) {
        item.parents('li').toggleClass(ClassName.TODO_LIST_DONE);
        if (!$(item).prop('checked')) {
          this.unCheck($(item));
          return;
        }
        this.check(item);
      }
      check(item) {
        this._config.onCheck.call(item);
      }
      unCheck(item) {
        this._config.onUnCheck.call(item);
      }

      // Private

      _init() {
        var that = this;
        $(Selector.DATA_TOGGLE).find('input:checkbox:checked').parents('li').toggleClass(ClassName.TODO_LIST_DONE);
        $(Selector.DATA_TOGGLE).on('change', 'input:checkbox', event => {
          that.toggle($(event.target));
        });
      }

      // Static

      static _jQueryInterface(config) {
        return this.each(function () {
          let data = $(this).data(DATA_KEY);
          const _config = $.extend({}, Default, $(this).data());
          if (!data) {
            data = new TodoList($(this), _config);
            $(this).data(DATA_KEY, data);
          }
          if (config === 'init') {
            data[config]();
          }
        });
      }
    }

    /**
     * Data API
     * ====================================================
     */

    $(window).on('load', () => {
      TodoList._jQueryInterface.call($(Selector.DATA_TOGGLE));
    });

    /**
     * jQuery API
     * ====================================================
     */

    $.fn[NAME] = TodoList._jQueryInterface;
    $.fn[NAME].Constructor = TodoList;
    $.fn[NAME].noConflict = function () {
      $.fn[NAME] = JQUERY_NO_CONFLICT;
      return TodoList._jQueryInterface;
    };
    return TodoList;
  })(jQuery);

  /**
   * --------------------------------------------
   * AdminLTE CardWidget.js
   * License MIT
   * --------------------------------------------
   */

  const CardWidget = ($ => {
    /**
     * Constants
     * ====================================================
     */

    const NAME = 'CardWidget';
    const DATA_KEY = 'lte.cardwidget';
    const EVENT_KEY = `.${DATA_KEY}`;
    const JQUERY_NO_CONFLICT = $.fn[NAME];
    const Event = {
      EXPANDED: `expanded${EVENT_KEY}`,
      COLLAPSED: `collapsed${EVENT_KEY}`,
      MAXIMIZED: `maximized${EVENT_KEY}`,
      MINIMIZED: `minimized${EVENT_KEY}`,
      REMOVED: `removed${EVENT_KEY}`
    };
    const ClassName = {
      CARD: 'card',
      COLLAPSED: 'collapsed-card',
      WAS_COLLAPSED: 'was-collapsed',
      MAXIMIZED: 'maximized-card'
    };
    const Selector = {
      DATA_REMOVE: '[data-card-widget="remove"]',
      DATA_COLLAPSE: '[data-card-widget="collapse"]',
      DATA_MAXIMIZE: '[data-card-widget="maximize"]',
      CARD: `.${ClassName.CARD}`,
      CARD_HEADER: '.card-header',
      CARD_BODY: '.card-body',
      CARD_FOOTER: '.card-footer',
      COLLAPSED: `.${ClassName.COLLAPSED}`
    };
    const Default = {
      animationSpeed: 'normal',
      collapseTrigger: Selector.DATA_COLLAPSE,
      removeTrigger: Selector.DATA_REMOVE,
      maximizeTrigger: Selector.DATA_MAXIMIZE,
      collapseIcon: 'fa-minus',
      expandIcon: 'fa-plus',
      maximizeIcon: 'fa-expand',
      minimizeIcon: 'fa-compress'
    };
    class CardWidget {
      constructor(element, settings) {
        this._element = element;
        this._parent = element.parents(Selector.CARD).first();
        if (element.hasClass(ClassName.CARD)) {
          this._parent = element;
        }
        this._settings = $.extend({}, Default, settings);
      }
      collapse() {
        this._parent.children(`${Selector.CARD_BODY}, ${Selector.CARD_FOOTER}`).slideUp(this._settings.animationSpeed, () => {
          this._parent.addClass(ClassName.COLLAPSED);
        });
        this._parent.find(this._settings.collapseTrigger + ' .' + this._settings.collapseIcon).addClass(this._settings.expandIcon).removeClass(this._settings.collapseIcon);
        const collapsed = $.Event(Event.COLLAPSED);
        this._element.trigger(collapsed, this._parent);
      }
      expand() {
        this._parent.children(`${Selector.CARD_BODY}, ${Selector.CARD_FOOTER}`).slideDown(this._settings.animationSpeed, () => {
          this._parent.removeClass(ClassName.COLLAPSED);
        });
        this._parent.find(this._settings.collapseTrigger + ' .' + this._settings.expandIcon).addClass(this._settings.collapseIcon).removeClass(this._settings.expandIcon);
        const expanded = $.Event(Event.EXPANDED);
        this._element.trigger(expanded, this._parent);
      }
      remove() {
        this._parent.slideUp();
        const removed = $.Event(Event.REMOVED);
        this._element.trigger(removed, this._parent);
      }
      toggle() {
        if (this._parent.hasClass(ClassName.COLLAPSED)) {
          this.expand();
          return;
        }
        this.collapse();
      }
      maximize() {
        this._parent.find(this._settings.maximizeTrigger + ' .' + this._settings.maximizeIcon).addClass(this._settings.minimizeIcon).removeClass(this._settings.maximizeIcon);
        this._parent.css({
          'height': this._parent.height(),
          'width': this._parent.width(),
          'transition': 'all .15s'
        }).delay(150).queue(function () {
          $(this).addClass(ClassName.MAXIMIZED);
          $('html').addClass(ClassName.MAXIMIZED);
          if ($(this).hasClass(ClassName.COLLAPSED)) {
            $(this).addClass(ClassName.WAS_COLLAPSED);
          }
          $(this).dequeue();
        });
        const maximized = $.Event(Event.MAXIMIZED);
        this._element.trigger(maximized, this._parent);
      }
      minimize() {
        this._parent.find(this._settings.maximizeTrigger + ' .' + this._settings.minimizeIcon).addClass(this._settings.maximizeIcon).removeClass(this._settings.minimizeIcon);
        this._parent.css('cssText', 'height:' + this._parent[0].style.height + ' !important;' + 'width:' + this._parent[0].style.width + ' !important; transition: all .15s;').delay(10).queue(function () {
          $(this).removeClass(ClassName.MAXIMIZED);
          $('html').removeClass(ClassName.MAXIMIZED);
          $(this).css({
            'height': 'inherit',
            'width': 'inherit'
          });
          if ($(this).hasClass(ClassName.WAS_COLLAPSED)) {
            $(this).removeClass(ClassName.WAS_COLLAPSED);
          }
          $(this).dequeue();
        });
        const MINIMIZED = $.Event(Event.MINIMIZED);
        this._element.trigger(MINIMIZED, this._parent);
      }
      toggleMaximize() {
        if (this._parent.hasClass(ClassName.MAXIMIZED)) {
          this.minimize();
          return;
        }
        this.maximize();
      }

      // Private

      _init(card) {
        this._parent = card;
        $(this).find(this._settings.collapseTrigger).click(() => {
          this.toggle();
        });
        $(this).find(this._settings.maximizeTrigger).click(() => {
          this.toggleMaximize();
        });
        $(this).find(this._settings.removeTrigger).click(() => {
          this.remove();
        });
      }

      // Static

      static _jQueryInterface(config) {
        let data = $(this).data(DATA_KEY);
        if (!data) {
          data = new CardWidget($(this), data);
          $(this).data(DATA_KEY, typeof config === 'string' ? data : config);
        }
        if (typeof config === 'string' && config.match(/collapse|expand|remove|toggle|maximize|minimize|toggleMaximize/)) {
          data[config]();
        } else if (typeof config === 'object') {
          data._init($(this));
        }
      }
    }

    /**
     * Data API
     * ====================================================
     */

    $(document).on('click', Selector.DATA_COLLAPSE, function (event) {
      if (event) {
        event.preventDefault();
      }
      CardWidget._jQueryInterface.call($(this), 'toggle');
    });
    $(document).on('click', Selector.DATA_REMOVE, function (event) {
      if (event) {
        event.preventDefault();
      }
      CardWidget._jQueryInterface.call($(this), 'remove');
    });
    $(document).on('click', Selector.DATA_MAXIMIZE, function (event) {
      if (event) {
        event.preventDefault();
      }
      CardWidget._jQueryInterface.call($(this), 'toggleMaximize');
    });

    /**
     * jQuery API
     * ====================================================
     */

    $.fn[NAME] = CardWidget._jQueryInterface;
    $.fn[NAME].Constructor = CardWidget;
    $.fn[NAME].noConflict = function () {
      $.fn[NAME] = JQUERY_NO_CONFLICT;
      return CardWidget._jQueryInterface;
    };
    return CardWidget;
  })(jQuery);

  /**
   * --------------------------------------------
   * AdminLTE CardRefresh.js
   * License MIT
   * --------------------------------------------
   */

  const CardRefresh = ($ => {
    /**
     * Constants
     * ====================================================
     */

    const NAME = 'CardRefresh';
    const DATA_KEY = 'lte.cardrefresh';
    const EVENT_KEY = `.${DATA_KEY}`;
    const JQUERY_NO_CONFLICT = $.fn[NAME];
    const Event = {
      LOADED: `loaded${EVENT_KEY}`,
      OVERLAY_ADDED: `overlay.added${EVENT_KEY}`,
      OVERLAY_REMOVED: `overlay.removed${EVENT_KEY}`
    };
    const ClassName = {
      CARD: 'card'
    };
    const Selector = {
      CARD: `.${ClassName.CARD}`,
      DATA_REFRESH: '[data-card-widget="card-refresh"]'
    };
    const Default = {
      source: '',
      sourceSelector: '',
      params: {},
      trigger: Selector.DATA_REFRESH,
      content: '.card-body',
      loadInContent: true,
      loadOnInit: true,
      responseType: '',
      overlayTemplate: '<div class="overlay"><i class="fas fa-2x fa-sync-alt fa-spin"></i></div>',
      onLoadStart: function () {},
      onLoadDone: function (response) {
        return response;
      }
    };
    class CardRefresh {
      constructor(element, settings) {
        this._element = element;
        this._parent = element.parents(Selector.CARD).first();
        this._settings = $.extend({}, Default, settings);
        this._overlay = $(this._settings.overlayTemplate);
        if (element.hasClass(ClassName.CARD)) {
          this._parent = element;
        }
        if (this._settings.source === '') {
          throw new Error('Source url was not defined. Please specify a url in your CardRefresh source option.');
        }
        this._init();
        if (this._settings.loadOnInit) {
          this.load();
        }
      }
      load() {
        this._addOverlay();
        this._settings.onLoadStart.call($(this));
        $.get(this._settings.source, this._settings.params, function (response) {
          if (this._settings.loadInContent) {
            if (this._settings.sourceSelector != '') {
              response = $(response).find(this._settings.sourceSelector).html();
            }
            this._parent.find(this._settings.content).html(response);
          }
          this._settings.onLoadDone.call($(this), response);
          this._removeOverlay();
        }.bind(this), this._settings.responseType !== '' && this._settings.responseType);
        const loadedEvent = $.Event(Event.LOADED);
        $(this._element).trigger(loadedEvent);
      }
      _addOverlay() {
        this._parent.append(this._overlay);
        const overlayAddedEvent = $.Event(Event.OVERLAY_ADDED);
        $(this._element).trigger(overlayAddedEvent);
      }
      _removeOverlay() {
        this._parent.find(this._overlay).remove();
        const overlayRemovedEvent = $.Event(Event.OVERLAY_REMOVED);
        $(this._element).trigger(overlayRemovedEvent);
      }
      // Private

      _init(card) {
        $(this).find(this._settings.trigger).on('click', () => {
          this.load();
        });
      }

      // Static

      static _jQueryInterface(config) {
        let data = $(this).data(DATA_KEY);
        let options = $(this).data();
        if (!data) {
          data = new CardRefresh($(this), options);
          $(this).data(DATA_KEY, typeof config === 'string' ? data : config);
        }
        if (typeof config === 'string' && config.match(/load/)) {
          data[config]();
        } else if (typeof config === 'object') {
          data._init($(this));
        }
      }
    }

    /**
     * Data API
     * ====================================================
     */

    $(document).on('click', Selector.DATA_REFRESH, function (event) {
      if (event) {
        event.preventDefault();
      }
      CardRefresh._jQueryInterface.call($(this), 'load');
    });

    /**
     * jQuery API
     * ====================================================
     */

    $.fn[NAME] = CardRefresh._jQueryInterface;
    $.fn[NAME].Constructor = CardRefresh;
    $.fn[NAME].noConflict = function () {
      $.fn[NAME] = JQUERY_NO_CONFLICT;
      return CardRefresh._jQueryInterface;
    };
    return CardRefresh;
  })(jQuery);

  /**
   * --------------------------------------------
   * AdminLTE Dropdown.js
   * License MIT
   * --------------------------------------------
   */

  const Dropdown = ($ => {
    /**
     * Constants
     * ====================================================
     */

    const NAME = 'Dropdown';
    const DATA_KEY = 'lte.dropdown';
    const JQUERY_NO_CONFLICT = $.fn[NAME];
    const Selector = {
      DROPDOWN_MENU: 'ul.dropdown-menu',
      DROPDOWN_TOGGLE: '[data-toggle="dropdown"]'
    };
    const Default = {};

    /**
     * Class Definition
     * ====================================================
     */

    class Dropdown {
      constructor(element, config) {
        this._config = config;
        this._element = element;
      }

      // Public

      toggleSubmenu() {
        this._element.siblings().show().toggleClass("show");
        if (!this._element.next().hasClass('show')) {
          this._element.parents('.dropdown-menu').first().find('.show').removeClass("show").hide();
        }
        this._element.parents('li.nav-item.dropdown.show').on('hidden.bs.dropdown', function (e) {
          $('.dropdown-submenu .show').removeClass("show").hide();
        });
      }

      // Static

      static _jQueryInterface(config) {
        return this.each(function () {
          let data = $(this).data(DATA_KEY);
          const _config = $.extend({}, Default, $(this).data());
          if (!data) {
            data = new Dropdown($(this), _config);
            $(this).data(DATA_KEY, data);
          }
          if (config === 'toggleSubmenu') {
            data[config]();
          }
        });
      }
    }

    /**
     * Data API
     * ====================================================
     */

    $(Selector.DROPDOWN_MENU + ' ' + Selector.DROPDOWN_TOGGLE).on("click", function (event) {
      event.preventDefault();
      event.stopPropagation();
      Dropdown._jQueryInterface.call($(this), 'toggleSubmenu');
    });

    // $(Selector.SIDEBAR + ' a').on('focusin', () => {
    //   $(Selector.MAIN_SIDEBAR).addClass(ClassName.SIDEBAR_FOCUSED);
    // })

    // $(Selector.SIDEBAR + ' a').on('focusout', () => {
    //   $(Selector.MAIN_SIDEBAR).removeClass(ClassName.SIDEBAR_FOCUSED);
    // })

    /**
     * jQuery API
     * ====================================================
     */

    $.fn[NAME] = Dropdown._jQueryInterface;
    $.fn[NAME].Constructor = Dropdown;
    $.fn[NAME].noConflict = function () {
      $.fn[NAME] = JQUERY_NO_CONFLICT;
      return Dropdown._jQueryInterface;
    };
    return Dropdown;
  })(jQuery);

  exports.CardRefresh = CardRefresh;
  exports.CardWidget = CardWidget;
  exports.ControlSidebar = ControlSidebar;
  exports.DirectChat = DirectChat;
  exports.Dropdown = Dropdown;
  exports.Layout = Layout;
  exports.PushMenu = PushMenu;
  exports.TodoList = TodoList;
  exports.Treeview = Treeview;

  Object.defineProperty(exports, '__esModule', { value: true });

})));
//# sourceMappingURL=adminlte.js.map
