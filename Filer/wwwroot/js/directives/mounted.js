Vue.directive('mounted', {
    inserted(el, binding, vnode) {
        var func = vnode.context[binding.expression];
        if (func) func(el);
    }
});