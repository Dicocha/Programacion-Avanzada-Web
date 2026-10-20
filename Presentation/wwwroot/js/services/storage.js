(function (window) {
    "use strict";

    const TodoStorage = {
        keys: {
            filter: "todo.filter"
        },

        getFilter: function (defaultValue) {
            return this.get(this.keys.filter, defaultValue || "all");
        },
        
        setFilter: function (value) {
            this.set(this.keys.filter, value);
        },

        get: function (key, defaultValue) {
            try {
                const raw = window.localStorage.getItem(key);
                if (raw === null) {
                    return defaultValue;
                }
                return JSON.parse(raw);
            } catch (e) {
                return defaultValue;
            }
        },

        set: function (key, value) {
            try {
                window.localStorage.setItem(key, JSON.stringify(value));
            } catch (e) {
                // ignore
            }
        }
    };

    window.TodoStorage = TodoStorage;
})(window);
