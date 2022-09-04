Vue.directive('mounted', {
    inserted: function (el, binding, vnode) {
        var func = vnode.context[binding.expression];
        if (func) func(el);
    }
});