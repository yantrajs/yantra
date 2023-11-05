(function (w, j) {
    var l = "enumerator is disposed",
        q = "single:sequence contains more than one element.",
        a = false,
        b = null,
        e = true,
        g = {
            Identity: function (a) {
                return a;
            },
            True: function () {
                return e;
            },
            Blank: function () {},
        },
        i = { Boolean: typeof e, Number: typeof 0, String: typeof "", Object: typeof {}, Undefined: typeof j, Function: typeof function () {} },
        d = {
            createLambda: function (a) {
                if (a == b) return g.Identity;
                if (typeof a == i.String)
                    if (a == "") return g.Identity;
                    else if (a.indexOf("=>") == -1) {
                        var m = new RegExp("[$]+", "g"),
                            c = 0,
                            j;
                        while ((j = m.exec(a))) {
                            var e = j[0].length;
                            if (e > c) c = e;
                        }
                        for (var f = [], d = 1; d <= c; d++) {
                            for (var h = "", l = 0; l < d; l++) h += "$";
                            f.push(h);
                        }
                        var n = Array.prototype.join.call(f, ",");
                        return new Function(n, "return " + a);
                    } else {
                        var k = a.match(/^[(\\s]*([^()]*?)[)\\s]*=>(.*)/);
                        return new Function(k[1], "return " + k[2]);
                    }
                return a;
            },
            isIEnumerable: function (b) {
                if (typeof Enumerator !== i.Undefined)
                    try {
                        new Enumerator(b);
                        return e;
                    } catch (c) {}
                return a;
            },
            defineProperty:
                Object.defineProperties != b
                    ? function (c, b, d) {
                          Object.defineProperty(c, b, { enumerable: a, configurable: e, writable: e, value: d });
                      }
                    : function (b, a, c) {
                          b[a] = c;
                      },
            compare: function (a, b) {
                return a === b ? 0 : a > b ? 1 : -1;
            },
            dispose: function (a) {
                a != b && a.dispose();
            },
        },
        o = { Before: 0, Running: 1, After: 2 },
        f = function (d, f, g) {
            var c = new u(),
                b = o.Before;
            this.current = c.current;
            this.moveNext = function () {
                try {
                    switch (b) {
                        case o.Before:
                            b = o.Running;
                            d();
                        case o.Running:
                            if (f.apply(c)) return e;
                            else {
                                this.dispose();
                                return a;
                            }
                        case o.After:
                            return a;
                    }
                } catch (g) {
                    this.dispose();
                    throw g;
                }
            };
            this.dispose = function () {
                if (b != o.Running) return;
                try {
                    g();
                } finally {
                    b = o.After;
                }
            };
        },
        u = function () {
            var c = b;
            this.current = function () {
                return c;
            };
            this.yieldReturn = function (a) {
                c = a;
                return e;
            };
            this.yieldBreak = function () {
                return a;
            };
        },
        c = function (a) {
            this.getEnumerator = a;
        };
    c.Utils = {};
    c.Utils.createLambda = function (a) {
        return d.createLambda(a);
    };
    c.Utils.createEnumerable = function (a) {
        return new c(a);
    };
    c.Utils.createEnumerator = function (a, b, c) {
        return new f(a, b, c);
    };
    c.Utils.extendTo = function (i) {
        var e = i.prototype,
            f;
        if (i === Array) {
            f = h.prototype;
            d.defineProperty(e, "getSource", function () {
                return this;
            });
        } else {
            f = c.prototype;
            d.defineProperty(e, "getEnumerator", function () {
                return c.from(this).getEnumerator();
            });
        }
        for (var a in f) {
            var g = f[a];
            if (e[a] == g) continue;
            if (e[a] != b) {
                a = a + "ByLinq";
                if (e[a] == g) continue;
            }
            g instanceof Function && d.defineProperty(e, a, g);
        }
    };
    c.choice = function () {
        var a = arguments;
        return new c(function () {
            return new f(
                function () {
                    a = a[0] instanceof Array ? a[0] : a[0].getEnumerator != b ? a[0].toArray() : a;
                },
                function () {
                    return this.yieldReturn(a[Math.floor(Math.random() * a.length)]);
                },
                g.Blank
            );
        });
    };
    c.cycle = function () {
        var a = arguments;
        return new c(function () {
            var c = 0;
            return new f(
                function () {
                    a = a[0] instanceof Array ? a[0] : a[0].getEnumerator != b ? a[0].toArray() : a;
                },
                function () {
                    if (c >= a.length) c = 0;
                    return this.yieldReturn(a[c++]);
                },
                g.Blank
            );
        });
    };
    c.empty = function () {
        return new c(function () {
            return new f(
                g.Blank,
                function () {
                    return a;
                },
                g.Blank
            );
        });
    };
    (c.from = function (j) {
        if (j == b) return c.empty();
        if (j instanceof c) return j;
        if (typeof j == i.Number || typeof j == i.Boolean) return c.repeat(j, 1);
        if (typeof j == i.String)
            return new c(function () {
                var b = 0;
                return new f(
                    g.Blank,
                    function () {
                        return b < j.length ? this.yieldReturn(j.charAt(b++)) : a;
                    },
                    g.Blank
                );
            });
        if (typeof j != i.Function) {
            if (typeof j.length == i.Number) return new h(j);
            if (!(j instanceof Object) && d.isIEnumerable(j))
                return new c(function () {
                    var c = e,
                        b;
                    return new f(
                        function () {
                            b = new Enumerator(j);
                        },
                        function () {
                            if (c) c = a;
                            else b.moveNext();
                            return b.atEnd() ? a : this.yieldReturn(b.item());
                        },
                        g.Blank
                    );
                });
            if (typeof Windows === i.Object && typeof j.first === i.Function)
                return new c(function () {
                    var c = e,
                        b;
                    return new f(
                        function () {
                            b = j.first();
                        },
                        function () {
                            if (c) c = a;
                            else b.moveNext();
                            return b.hasCurrent ? this.yieldReturn(b.current) : this.yieldBreak();
                        },
                        g.Blank
                    );
                });
        }
        return new c(function () {
            var b = [],
                c = 0;
            return new f(
                function () {
                    for (var a in j) {
                        var c = j[a];
                        !(c instanceof Function) && Object.prototype.hasOwnProperty.call(j, a) && b.push({ key: a, value: c });
                    }
                },
                function () {
                    return c < b.length ? this.yieldReturn(b[c++]) : a;
                },
                g.Blank
            );
        });
    }),
        (c.make = function (a) {
            return c.repeat(a, 1);
        });
    c.matches = function (h, e, d) {
        if (d == b) d = "";
        if (e instanceof RegExp) {
            d += e.ignoreCase ? "i" : "";
            d += e.multiline ? "m" : "";
            e = e.source;
        }
        if (d.indexOf("g") === -1) d += "g";
        return new c(function () {
            var b;
            return new f(
                function () {
                    b = new RegExp(e, d);
                },
                function () {
                    var c = b.exec(h);
                    return c ? this.yieldReturn(c) : a;
                },
                g.Blank
            );
        });
    };
    c.range = function (e, d, a) {
        if (a == b) a = 1;
        return new c(function () {
            var b,
                c = 0;
            return new f(
                function () {
                    b = e - a;
                },
                function () {
                    return c++ < d ? this.yieldReturn((b += a)) : this.yieldBreak();
                },
                g.Blank
            );
        });
    };
    c.rangeDown = function (e, d, a) {
        if (a == b) a = 1;
        return new c(function () {
            var b,
                c = 0;
            return new f(
                function () {
                    b = e + a;
                },
                function () {
                    return c++ < d ? this.yieldReturn((b -= a)) : this.yieldBreak();
                },
                g.Blank
            );
        });
    };
    c.rangeTo = function (d, e, a) {
        if (a == b) a = 1;
        return d < e
            ? new c(function () {
                  var b;
                  return new f(
                      function () {
                          b = d - a;
                      },
                      function () {
                          var c = (b += a);
                          return c <= e ? this.yieldReturn(c) : this.yieldBreak();
                      },
                      g.Blank
                  );
              })
            : new c(function () {
                  var b;
                  return new f(
                      function () {
                          b = d + a;
                      },
                      function () {
                          var c = (b -= a);
                          return c >= e ? this.yieldReturn(c) : this.yieldBreak();
                      },
                      g.Blank
                  );
              });
    };
    c.repeat = function (a, d) {
        return d != b
            ? c.repeat(a).take(d)
            : new c(function () {
                  return new f(
                      g.Blank,
                      function () {
                          return this.yieldReturn(a);
                      },
                      g.Blank
                  );
              });
    };
    c.repeatWithFinalize = function (a, e) {
        a = d.createLambda(a);
        e = d.createLambda(e);
        return new c(function () {
            var c;
            return new f(
                function () {
                    c = a();
                },
                function () {
                    return this.yieldReturn(c);
                },
                function () {
                    if (c != b) {
                        e(c);
                        c = b;
                    }
                }
            );
        });
    };
    c.generate = function (a, e) {
        if (e != b) return c.generate(a).take(e);
        a = d.createLambda(a);
        return new c(function () {
            return new f(
                g.Blank,
                function () {
                    return this.yieldReturn(a());
                },
                g.Blank
            );
        });
    };
    c.toInfinity = function (d, a) {
        if (d == b) d = 0;
        if (a == b) a = 1;
        return new c(function () {
            var b;
            return new f(
                function () {
                    b = d - a;
                },
                function () {
                    return this.yieldReturn((b += a));
                },
                g.Blank
            );
        });
    };
    c.toNegativeInfinity = function (d, a) {
        if (d == b) d = 0;
        if (a == b) a = 1;
        return new c(function () {
            var b;
            return new f(
                function () {
                    b = d + a;
                },
                function () {
                    return this.yieldReturn((b -= a));
                },
                g.Blank
            );
        });
    };
    c.unfold = function (h, b) {
        b = d.createLambda(b);
        return new c(function () {
            var d = e,
                c;
            return new f(
                g.Blank,
                function () {
                    if (d) {
                        d = a;
                        c = h;
                        return this.yieldReturn(c);
                    }
                    c = b(c);
                    return this.yieldReturn(c);
                },
                g.Blank
            );
        });
    };
    c.defer = function (a) {
        return new c(function () {
            var b;
            return new f(
                function () {
                    b = c.from(a()).getEnumerator();
                },
                function () {
                    return b.moveNext() ? this.yieldReturn(b.current()) : this.yieldBreak();
                },
                function () {
                    d.dispose(b);
                }
            );
        });
    };
    c.prototype.traverseBreadthFirst = function (g, b) {
        var h = this;
        g = d.createLambda(g);
        b = d.createLambda(b);
        return new c(function () {
            var i,
                k = 0,
                j = [];
            return new f(
                function () {
                    i = h.getEnumerator();
                },
                function () {
                    while (e) {
                        if (i.moveNext()) {
                            j.push(i.current());
                            return this.yieldReturn(b(i.current(), k));
                        }
                        var f = c.from(j).selectMany(function (a) {
                            return g(a);
                        });
                        if (!f.any()) return a;
                        else {
                            k++;
                            j = [];
                            d.dispose(i);
                            i = f.getEnumerator();
                        }
                    }
                },
                function () {
                    d.dispose(i);
                }
            );
        });
    };
    c.prototype.traverseDepthFirst = function (g, b) {
        var h = this;
        g = d.createLambda(g);
        b = d.createLambda(b);
        return new c(function () {
            var j = [],
                i;
            return new f(
                function () {
                    i = h.getEnumerator();
                },
                function () {
                    while (e) {
                        if (i.moveNext()) {
                            var f = b(i.current(), j.length);
                            j.push(i);
                            i = c.from(g(i.current())).getEnumerator();
                            return this.yieldReturn(f);
                        }
                        if (j.length <= 0) return a;
                        d.dispose(i);
                        i = j.pop();
                    }
                },
                function () {
                    try {
                        d.dispose(i);
                    } finally {
                        c.from(j).forEach(function (a) {
                            a.dispose();
                        });
                    }
                }
            );
        });
    };
    c.prototype.flatten = function () {
        var h = this;
        return new c(function () {
            var j,
                i = b;
            return new f(
                function () {
                    j = h.getEnumerator();
                },
                function () {
                    while (e) {
                        if (i != b)
                            if (i.moveNext()) return this.yieldReturn(i.current());
                            else i = b;
                        if (j.moveNext())
                            if (j.current() instanceof Array) {
                                d.dispose(i);
                                i = c.from(j.current()).selectMany(g.Identity).flatten().getEnumerator();
                                continue;
                            } else return this.yieldReturn(j.current());
                        return a;
                    }
                },
                function () {
                    try {
                        d.dispose(j);
                    } finally {
                        d.dispose(i);
                    }
                }
            );
        });
    };
    c.prototype.pairwise = function (b) {
        var e = this;
        b = d.createLambda(b);
        return new c(function () {
            var c;
            return new f(
                function () {
                    c = e.getEnumerator();
                    c.moveNext();
                },
                function () {
                    var d = c.current();
                    return c.moveNext() ? this.yieldReturn(b(d, c.current())) : a;
                },
                function () {
                    d.dispose(c);
                }
            );
        });
    };
    c.prototype.scan = function (i, g) {
        var h;
        if (g == b) {
            g = d.createLambda(i);
            h = a;
        } else {
            g = d.createLambda(g);
            h = e;
        }
        var j = this;
        return new c(function () {
            var b,
                c,
                k = e;
            return new f(
                function () {
                    b = j.getEnumerator();
                },
                function () {
                    if (k) {
                        k = a;
                        if (!h) {
                            if (b.moveNext()) return this.yieldReturn((c = b.current()));
                        } else return this.yieldReturn((c = i));
                    }
                    return b.moveNext() ? this.yieldReturn((c = g(c, b.current()))) : a;
                },
                function () {
                    d.dispose(b);
                }
            );
        });
    };
    c.prototype.select = function (e) {
        e = d.createLambda(e);
        if (e.length <= 1) return new m(this, b, e);
        else {
            var g = this;
            return new c(function () {
                var b,
                    c = 0;
                return new f(
                    function () {
                        b = g.getEnumerator();
                    },
                    function () {
                        return b.moveNext() ? this.yieldReturn(e(b.current(), c++)) : a;
                    },
                    function () {
                        d.dispose(b);
                    }
                );
            });
        }
    };
    c.prototype.selectMany = function (g, e) {
        var h = this;
        g = d.createLambda(g);
        if (e == b)
            e = function (b, a) {
                return a;
            };
        e = d.createLambda(e);
        return new c(function () {
            var k,
                i = j,
                l = 0;
            return new f(
                function () {
                    k = h.getEnumerator();
                },
                function () {
                    if (i === j) if (!k.moveNext()) return a;
                    do {
                        if (i == b) {
                            var f = g(k.current(), l++);
                            i = c.from(f).getEnumerator();
                        }
                        if (i.moveNext()) return this.yieldReturn(e(k.current(), i.current()));
                        d.dispose(i);
                        i = b;
                    } while (k.moveNext());
                    return a;
                },
                function () {
                    try {
                        d.dispose(k);
                    } finally {
                        d.dispose(i);
                    }
                }
            );
        });
    };
    c.prototype.where = function (b) {
        b = d.createLambda(b);
        if (b.length <= 1) return new n(this, b);
        else {
            var e = this;
            return new c(function () {
                var c,
                    g = 0;
                return new f(
                    function () {
                        c = e.getEnumerator();
                    },
                    function () {
                        while (c.moveNext()) if (b(c.current(), g++)) return this.yieldReturn(c.current());
                        return a;
                    },
                    function () {
                        d.dispose(c);
                    }
                );
            });
        }
    };
    c.prototype.choose = function (a) {
        a = d.createLambda(a);
        var e = this;
        return new c(function () {
            var c,
                g = 0;
            return new f(
                function () {
                    c = e.getEnumerator();
                },
                function () {
                    while (c.moveNext()) {
                        var d = a(c.current(), g++);
                        if (d != b) return this.yieldReturn(d);
                    }
                    return this.yieldBreak();
                },
                function () {
                    d.dispose(c);
                }
            );
        });
    };
    c.prototype.ofType = function (c) {
        var a;
        switch (c) {
            case Number:
                a = i.Number;
                break;
            case String:
                a = i.String;
                break;
            case Boolean:
                a = i.Boolean;
                break;
            case Function:
                a = i.Function;
                break;
            default:
                a = b;
        }
        return a === b
            ? this.where(function (a) {
                  return a instanceof c;
              })
            : this.where(function (b) {
                  return typeof b === a;
              });
    };
    c.prototype.zip = function () {
        var i = arguments,
            e = d.createLambda(arguments[arguments.length - 1]),
            g = this;
        if (arguments.length == 2) {
            var h = arguments[0];
            return new c(function () {
                var i,
                    b,
                    j = 0;
                return new f(
                    function () {
                        i = g.getEnumerator();
                        b = c.from(h).getEnumerator();
                    },
                    function () {
                        return i.moveNext() && b.moveNext() ? this.yieldReturn(e(i.current(), b.current(), j++)) : a;
                    },
                    function () {
                        try {
                            d.dispose(i);
                        } finally {
                            d.dispose(b);
                        }
                    }
                );
            });
        } else
            return new c(function () {
                var a,
                    h = 0;
                return new f(
                    function () {
                        var b = c
                            .make(g)
                            .concat(c.from(i).takeExceptLast().select(c.from))
                            .select(function (a) {
                                return a.getEnumerator();
                            })
                            .toArray();
                        a = c.from(b);
                    },
                    function () {
                        if (
                            a.all(function (a) {
                                return a.moveNext();
                            })
                        ) {
                            var c = a
                                .select(function (a) {
                                    return a.current();
                                })
                                .toArray();
                            c.push(h++);
                            return this.yieldReturn(e.apply(b, c));
                        } else return this.yieldBreak();
                    },
                    function () {
                        c.from(a).forEach(d.dispose);
                    }
                );
            });
    };
    c.prototype.merge = function () {
        var b = arguments,
            a = this;
        return new c(function () {
            var e,
                g = -1;
            return new f(
                function () {
                    e = c
                        .make(a)
                        .concat(c.from(b).select(c.from))
                        .select(function (a) {
                            return a.getEnumerator();
                        })
                        .toArray();
                },
                function () {
                    while (e.length > 0) {
                        g = g >= e.length - 1 ? 0 : g + 1;
                        var a = e[g];
                        if (a.moveNext()) return this.yieldReturn(a.current());
                        else {
                            a.dispose();
                            e.splice(g--, 1);
                        }
                    }
                    return this.yieldBreak();
                },
                function () {
                    c.from(e).forEach(d.dispose);
                }
            );
        });
    };
    c.prototype.join = function (n, i, h, l, k) {
        i = d.createLambda(i);
        h = d.createLambda(h);
        l = d.createLambda(l);
        k = d.createLambda(k);
        var m = this;
        return new c(function () {
            var o,
                r,
                p = b,
                q = 0;
            return new f(
                function () {
                    o = m.getEnumerator();
                    r = c.from(n).toLookup(h, g.Identity, k);
                },
                function () {
                    while (e) {
                        if (p != b) {
                            var c = p[q++];
                            if (c !== j) return this.yieldReturn(l(o.current(), c));
                            c = b;
                            q = 0;
                        }
                        if (o.moveNext()) {
                            var d = i(o.current());
                            p = r.get(d).toArray();
                        } else return a;
                    }
                },
                function () {
                    d.dispose(o);
                }
            );
        });
    };
    c.prototype.groupJoin = function (l, h, e, j, i) {
        h = d.createLambda(h);
        e = d.createLambda(e);
        j = d.createLambda(j);
        i = d.createLambda(i);
        var k = this;
        return new c(function () {
            var m = k.getEnumerator(),
                n = b;
            return new f(
                function () {
                    m = k.getEnumerator();
                    n = c.from(l).toLookup(e, g.Identity, i);
                },
                function () {
                    if (m.moveNext()) {
                        var b = n.get(h(m.current()));
                        return this.yieldReturn(j(m.current(), b));
                    }
                    return a;
                },
                function () {
                    d.dispose(m);
                }
            );
        });
    };
    c.prototype.all = function (b) {
        b = d.createLambda(b);
        var c = e;
        this.forEach(function (d) {
            if (!b(d)) {
                c = a;
                return a;
            }
        });
        return c;
    };
    c.prototype.any = function (c) {
        c = d.createLambda(c);
        var b = this.getEnumerator();
        try {
            if (arguments.length == 0) return b.moveNext();
            while (b.moveNext()) if (c(b.current())) return e;
            return a;
        } finally {
            d.dispose(b);
        }
    };
    c.prototype.isEmpty = function () {
        return !this.any();
    };
    c.prototype.concat = function () {
        var e = this;
        if (arguments.length == 1) {
            var g = arguments[0];
            return new c(function () {
                var i, h;
                return new f(
                    function () {
                        i = e.getEnumerator();
                    },
                    function () {
                        if (h == b) {
                            if (i.moveNext()) return this.yieldReturn(i.current());
                            h = c.from(g).getEnumerator();
                        }
                        return h.moveNext() ? this.yieldReturn(h.current()) : a;
                    },
                    function () {
                        try {
                            d.dispose(i);
                        } finally {
                            d.dispose(h);
                        }
                    }
                );
            });
        } else {
            var h = arguments;
            return new c(function () {
                var a;
                return new f(
                    function () {
                        a = c
                            .make(e)
                            .concat(c.from(h).select(c.from))
                            .select(function (a) {
                                return a.getEnumerator();
                            })
                            .toArray();
                    },
                    function () {
                        while (a.length > 0) {
                            var b = a[0];
                            if (b.moveNext()) return this.yieldReturn(b.current());
                            else {
                                b.dispose();
                                a.splice(0, 1);
                            }
                        }
                        return this.yieldBreak();
                    },
                    function () {
                        c.from(a).forEach(d.dispose);
                    }
                );
            });
        }
    };
    c.prototype.insert = function (h, b) {
        var g = this;
        return new c(function () {
            var j,
                i,
                l = 0,
                k = a;
            return new f(
                function () {
                    j = g.getEnumerator();
                    i = c.from(b).getEnumerator();
                },
                function () {
                    if (l == h && i.moveNext()) {
                        k = e;
                        return this.yieldReturn(i.current());
                    }
                    if (j.moveNext()) {
                        l++;
                        return this.yieldReturn(j.current());
                    }
                    return !k && i.moveNext() ? this.yieldReturn(i.current()) : a;
                },
                function () {
                    try {
                        d.dispose(j);
                    } finally {
                        d.dispose(i);
                    }
                }
            );
        });
    };
    c.prototype.alternate = function (a) {
        var g = this;
        return new c(function () {
            var j, i, k, h;
            return new f(
                function () {
                    if (a instanceof Array || a.getEnumerator != b) k = c.from(c.from(a).toArray());
                    else k = c.make(a);
                    i = g.getEnumerator();
                    if (i.moveNext()) j = i.current();
                },
                function () {
                    while (e) {
                        if (h != b)
                            if (h.moveNext()) return this.yieldReturn(h.current());
                            else h = b;
                        if (j == b && i.moveNext()) {
                            j = i.current();
                            h = k.getEnumerator();
                            continue;
                        } else if (j != b) {
                            var a = j;
                            j = b;
                            return this.yieldReturn(a);
                        }
                        return this.yieldBreak();
                    }
                },
                function () {
                    try {
                        d.dispose(i);
                    } finally {
                        d.dispose(h);
                    }
                }
            );
        });
    };
    c.prototype.contains = function (f, b) {
        b = d.createLambda(b);
        var c = this.getEnumerator();
        try {
            while (c.moveNext()) if (b(c.current()) === f) return e;
            return a;
        } finally {
            d.dispose(c);
        }
    };
    c.prototype.defaultIfEmpty = function (g) {
        var h = this;
        if (g === j) g = b;
        return new c(function () {
            var b,
                c = e;
            return new f(
                function () {
                    b = h.getEnumerator();
                },
                function () {
                    if (b.moveNext()) {
                        c = a;
                        return this.yieldReturn(b.current());
                    } else if (c) {
                        c = a;
                        return this.yieldReturn(g);
                    }
                    return a;
                },
                function () {
                    d.dispose(b);
                }
            );
        });
    };
    c.prototype.distinct = function (a) {
        return this.except(c.empty(), a);
    };
    c.prototype.distinctUntilChanged = function (b) {
        b = d.createLambda(b);
        var e = this;
        return new c(function () {
            var c, g, h;
            return new f(
                function () {
                    c = e.getEnumerator();
                },
                function () {
                    while (c.moveNext()) {
                        var d = b(c.current());
                        if (h) {
                            h = a;
                            g = d;
                            return this.yieldReturn(c.current());
                        }
                        if (g === d) continue;
                        g = d;
                        return this.yieldReturn(c.current());
                    }
                    return this.yieldBreak();
                },
                function () {
                    d.dispose(c);
                }
            );
        });
    };
    c.prototype.except = function (e, b) {
        b = d.createLambda(b);
        var g = this;
        return new c(function () {
            var h, i;
            return new f(
                function () {
                    h = g.getEnumerator();
                    i = new r(b);
                    c.from(e).forEach(function (a) {
                        i.add(a);
                    });
                },
                function () {
                    while (h.moveNext()) {
                        var b = h.current();
                        if (!i.contains(b)) {
                            i.add(b);
                            return this.yieldReturn(b);
                        }
                    }
                    return a;
                },
                function () {
                    d.dispose(h);
                }
            );
        });
    };
    c.prototype.intersect = function (e, b) {
        b = d.createLambda(b);
        var g = this;
        return new c(function () {
            var h, i, j;
            return new f(
                function () {
                    h = g.getEnumerator();
                    i = new r(b);
                    c.from(e).forEach(function (a) {
                        i.add(a);
                    });
                    j = new r(b);
                },
                function () {
                    while (h.moveNext()) {
                        var b = h.current();
                        if (!j.contains(b) && i.contains(b)) {
                            j.add(b);
                            return this.yieldReturn(b);
                        }
                    }
                    return a;
                },
                function () {
                    d.dispose(h);
                }
            );
        });
    };
    c.prototype.sequenceEqual = function (h, f) {
        f = d.createLambda(f);
        var g = this.getEnumerator();
        try {
            var b = c.from(h).getEnumerator();
            try {
                while (g.moveNext()) if (!b.moveNext() || f(g.current()) !== f(b.current())) return a;
                return b.moveNext() ? a : e;
            } finally {
                d.dispose(b);
            }
        } finally {
            d.dispose(g);
        }
    };
    c.prototype.union = function (e, b) {
        b = d.createLambda(b);
        var g = this;
        return new c(function () {
            var k, h, i;
            return new f(
                function () {
                    k = g.getEnumerator();
                    i = new r(b);
                },
                function () {
                    var b;
                    if (h === j) {
                        while (k.moveNext()) {
                            b = k.current();
                            if (!i.contains(b)) {
                                i.add(b);
                                return this.yieldReturn(b);
                            }
                        }
                        h = c.from(e).getEnumerator();
                    }
                    while (h.moveNext()) {
                        b = h.current();
                        if (!i.contains(b)) {
                            i.add(b);
                            return this.yieldReturn(b);
                        }
                    }
                    return a;
                },
                function () {
                    try {
                        d.dispose(k);
                    } finally {
                        d.dispose(h);
                    }
                }
            );
        });
    };
    c.prototype.orderBy = function (b) {
        return new k(this, b, a);
    };
    c.prototype.orderByDescending = function (a) {
        return new k(this, a, e);
    };
    c.prototype.reverse = function () {
        var b = this;
        return new c(function () {
            var c, d;
            return new f(
                function () {
                    c = b.toArray();
                    d = c.length;
                },
                function () {
                    return d > 0 ? this.yieldReturn(c[--d]) : a;
                },
                g.Blank
            );
        });
    };
    c.prototype.shuffle = function () {
        var b = this;
        return new c(function () {
            var c;
            return new f(
                function () {
                    c = b.toArray();
                },
                function () {
                    if (c.length > 0) {
                        var b = Math.floor(Math.random() * c.length);
                        return this.yieldReturn(c.splice(b, 1)[0]);
                    }
                    return a;
                },
                g.Blank
            );
        });
    };
    c.prototype.weightedSample = function (a) {
        a = d.createLambda(a);
        var e = this;
        return new c(function () {
            var c,
                d = 0;
            return new f(
                function () {
                    c = e
                        .choose(function (e) {
                            var c = a(e);
                            if (c <= 0) return b;
                            d += c;
                            return { value: e, bound: d };
                        })
                        .toArray();
                },
                function () {
                    if (c.length > 0) {
                        var f = Math.floor(Math.random() * d) + 1,
                            e = -1,
                            a = c.length;
                        while (a - e > 1) {
                            var b = Math.floor((e + a) / 2);
                            if (c[b].bound >= f) a = b;
                            else e = b;
                        }
                        return this.yieldReturn(c[a].value);
                    }
                    return this.yieldBreak();
                },
                g.Blank
            );
        });
    };
    c.prototype.groupBy = function (i, h, e, g) {
        var j = this;
        i = d.createLambda(i);
        h = d.createLambda(h);
        if (e != b) e = d.createLambda(e);
        g = d.createLambda(g);
        return new c(function () {
            var c;
            return new f(
                function () {
                    c = j.toLookup(i, h, g).toEnumerable().getEnumerator();
                },
                function () {
                    while (c.moveNext()) return e == b ? this.yieldReturn(c.current()) : this.yieldReturn(e(c.current().key(), c.current()));
                    return a;
                },
                function () {
                    d.dispose(c);
                }
            );
        });
    };
    c.prototype.partitionBy = function (j, i, g, h) {
        var l = this;
        j = d.createLambda(j);
        i = d.createLambda(i);
        h = d.createLambda(h);
        var k;
        if (g == b) {
            k = a;
            g = function (b, a) {
                return new t(b, a);
            };
        } else {
            k = e;
            g = d.createLambda(g);
        }
        return new c(function () {
            var b,
                n,
                o,
                m = [];
            return new f(
                function () {
                    b = l.getEnumerator();
                    if (b.moveNext()) {
                        n = j(b.current());
                        o = h(n);
                        m.push(i(b.current()));
                    }
                },
                function () {
                    var d;
                    while ((d = b.moveNext()) == e)
                        if (o === h(j(b.current()))) m.push(i(b.current()));
                        else break;
                    if (m.length > 0) {
                        var f = k ? g(n, c.from(m)) : g(n, m);
                        if (d) {
                            n = j(b.current());
                            o = h(n);
                            m = [i(b.current())];
                        } else m = [];
                        return this.yieldReturn(f);
                    }
                    return a;
                },
                function () {
                    d.dispose(b);
                }
            );
        });
    };
    c.prototype.buffer = function (e) {
        var b = this;
        return new c(function () {
            var c;
            return new f(
                function () {
                    c = b.getEnumerator();
                },
                function () {
                    var b = [],
                        d = 0;
                    while (c.moveNext()) {
                        b.push(c.current());
                        if (++d >= e) return this.yieldReturn(b);
                    }
                    return b.length > 0 ? this.yieldReturn(b) : a;
                },
                function () {
                    d.dispose(c);
                }
            );
        });
    };
    c.prototype.aggregate = function (c, b, a) {
        a = d.createLambda(a);
        return a(this.scan(c, b, a).last());
    };
    c.prototype.average = function (a) {
        a = d.createLambda(a);
        var c = 0,
            b = 0;
        this.forEach(function (d) {
            c += a(d);
            ++b;
        });
        return c / b;
    };
    c.prototype.count = function (a) {
        a = a == b ? g.True : d.createLambda(a);
        var c = 0;
        this.forEach(function (d, b) {
            if (a(d, b)) ++c;
        });
        return c;
    };
    c.prototype.max = function (a) {
        if (a == b) a = g.Identity;
        return this.select(a).aggregate(function (a, b) {
            return a > b ? a : b;
        });
    };
    c.prototype.min = function (a) {
        if (a == b) a = g.Identity;
        return this.select(a).aggregate(function (a, b) {
            return a < b ? a : b;
        });
    };
    c.prototype.maxBy = function (a) {
        a = d.createLambda(a);
        return this.aggregate(function (b, c) {
            return a(b) > a(c) ? b : c;
        });
    };
    c.prototype.minBy = function (a) {
        a = d.createLambda(a);
        return this.aggregate(function (b, c) {
            return a(b) < a(c) ? b : c;
        });
    };
    c.prototype.sum = function (a) {
        if (a == b) a = g.Identity;
        return this.select(a).aggregate(0, function (a, b) {
            return a + b;
        });
    };
    c.prototype.elementAt = function (d) {
        var c,
            b = a;
        this.forEach(function (g, f) {
            if (f == d) {
                c = g;
                b = e;
                return a;
            }
        });
        if (!b) throw new Error("index is less than 0 or greater than or equal to the number of elements in source.");
        return c;
    };
    c.prototype.elementAtOrDefault = function (g, c) {
        if (c === j) c = b;
        var f,
            d = a;
        this.forEach(function (c, b) {
            if (b == g) {
                f = c;
                d = e;
                return a;
            }
        });
        return !d ? c : f;
    };
    c.prototype.first = function (c) {
        if (c != b) return this.where(c).first();
        var f,
            d = a;
        this.forEach(function (b) {
            f = b;
            d = e;
            return a;
        });
        if (!d) throw new Error("first:No element satisfies the condition.");
        return f;
    };
    c.prototype.firstOrDefault = function (d, c) {
        if (c === j) c = b;
        if (d != b) return this.where(d).firstOrDefault(b, c);
        var g,
            f = a;
        this.forEach(function (b) {
            g = b;
            f = e;
            return a;
        });
        return !f ? c : g;
    };
    c.prototype.last = function (c) {
        if (c != b) return this.where(c).last();
        var f,
            d = a;
        this.forEach(function (a) {
            d = e;
            f = a;
        });
        if (!d) throw new Error("last:No element satisfies the condition.");
        return f;
    };
    c.prototype.lastOrDefault = function (d, c) {
        if (c === j) c = b;
        if (d != b) return this.where(d).lastOrDefault(b, c);
        var g,
            f = a;
        this.forEach(function (a) {
            f = e;
            g = a;
        });
        return !f ? c : g;
    };
    c.prototype.single = function (d) {
        if (d != b) return this.where(d).single();
        var f,
            c = a;
        this.forEach(function (a) {
            if (!c) {
                c = e;
                f = a;
            } else throw new Error(q);
        });
        if (!c) throw new Error("single:No element satisfies the condition.");
        return f;
    };
    c.prototype.singleOrDefault = function (f, c) {
        if (c === j) c = b;
        if (f != b) return this.where(f).singleOrDefault(b, c);
        var g,
            d = a;
        this.forEach(function (a) {
            if (!d) {
                d = e;
                g = a;
            } else throw new Error(q);
        });
        return !d ? c : g;
    };
    c.prototype.skip = function (e) {
        var b = this;
        return new c(function () {
            var c,
                g = 0;
            return new f(
                function () {
                    c = b.getEnumerator();
                    while (g++ < e && c.moveNext());
                },
                function () {
                    return c.moveNext() ? this.yieldReturn(c.current()) : a;
                },
                function () {
                    d.dispose(c);
                }
            );
        });
    };
    c.prototype.skipWhile = function (b) {
        b = d.createLambda(b);
        var g = this;
        return new c(function () {
            var c,
                i = 0,
                h = a;
            return new f(
                function () {
                    c = g.getEnumerator();
                },
                function () {
                    while (!h)
                        if (c.moveNext()) {
                            if (!b(c.current(), i++)) {
                                h = e;
                                return this.yieldReturn(c.current());
                            }
                            continue;
                        } else return a;
                    return c.moveNext() ? this.yieldReturn(c.current()) : a;
                },
                function () {
                    d.dispose(c);
                }
            );
        });
    };
    c.prototype.take = function (e) {
        var b = this;
        return new c(function () {
            var c,
                g = 0;
            return new f(
                function () {
                    c = b.getEnumerator();
                },
                function () {
                    return g++ < e && c.moveNext() ? this.yieldReturn(c.current()) : a;
                },
                function () {
                    d.dispose(c);
                }
            );
        });
    };
    c.prototype.takeWhile = function (b) {
        b = d.createLambda(b);
        var e = this;
        return new c(function () {
            var c,
                g = 0;
            return new f(
                function () {
                    c = e.getEnumerator();
                },
                function () {
                    return c.moveNext() && b(c.current(), g++) ? this.yieldReturn(c.current()) : a;
                },
                function () {
                    d.dispose(c);
                }
            );
        });
    };
    c.prototype.takeExceptLast = function (e) {
        if (e == b) e = 1;
        var g = this;
        return new c(function () {
            if (e <= 0) return g.getEnumerator();
            var b,
                c = [];
            return new f(
                function () {
                    b = g.getEnumerator();
                },
                function () {
                    while (b.moveNext()) {
                        if (c.length == e) {
                            c.push(b.current());
                            return this.yieldReturn(c.shift());
                        }
                        c.push(b.current());
                    }
                    return a;
                },
                function () {
                    d.dispose(b);
                }
            );
        });
    };
    c.prototype.takeFromLast = function (e) {
        if (e <= 0 || e == b) return c.empty();
        var g = this;
        return new c(function () {
            var j,
                h,
                i = [];
            return new f(
                function () {
                    j = g.getEnumerator();
                },
                function () {
                    while (j.moveNext()) {
                        i.length == e && i.shift();
                        i.push(j.current());
                    }
                    if (h == b) h = c.from(i).getEnumerator();
                    return h.moveNext() ? this.yieldReturn(h.current()) : a;
                },
                function () {
                    d.dispose(h);
                }
            );
        });
    };
    c.prototype.indexOf = function (d) {
        var c = b;
        if (typeof d === i.Function)
            this.forEach(function (e, b) {
                if (d(e, b)) {
                    c = b;
                    return a;
                }
            });
        else
            this.forEach(function (e, b) {
                if (e === d) {
                    c = b;
                    return a;
                }
            });
        return c !== b ? c : -1;
    };
    c.prototype.lastIndexOf = function (b) {
        var a = -1;
        if (typeof b === i.Function)
            this.forEach(function (d, c) {
                if (b(d, c)) a = c;
            });
        else
            this.forEach(function (d, c) {
                if (d === b) a = c;
            });
        return a;
    };
    c.prototype.asEnumerable = function () {
        return c.from(this);
    };
    c.prototype.toArray = function () {
        var a = [];
        this.forEach(function (b) {
            a.push(b);
        });
        return a;
    };
    c.prototype.toLookup = function (c, b, a) {
        c = d.createLambda(c);
        b = d.createLambda(b);
        a = d.createLambda(a);
        var e = new r(a);
        this.forEach(function (g) {
            var f = c(g),
                a = b(g),
                d = e.get(f);
            if (d !== j) d.push(a);
            else e.add(f, [a]);
        });
        return new v(e);
    };
    c.prototype.toObject = function (b, a) {
        b = d.createLambda(b);
        a = d.createLambda(a);
        var c = {};
        this.forEach(function (d) {
            c[b(d)] = a(d);
        });
        return c;
    };
    c.prototype.toDictionary = function (c, b, a) {
        c = d.createLambda(c);
        b = d.createLambda(b);
        a = d.createLambda(a);
        var e = new r(a);
        this.forEach(function (a) {
            e.add(c(a), b(a));
        });
        return e;
    };
    c.prototype.toJSONString = function (a, c) {
        if (typeof JSON === i.Undefined || JSON.stringify == b) throw new Error("toJSONString can't find JSON.stringify. This works native JSON support Browser or include json2.js");
        return JSON.stringify(this.toArray(), a, c);
    };
    c.prototype.toJoinedString = function (a, c) {
        if (a == b) a = "";
        if (c == b) c = g.Identity;
        return this.select(c).toArray().join(a);
    };
    c.prototype.doAction = function (b) {
        var e = this;
        b = d.createLambda(b);
        return new c(function () {
            var c,
                g = 0;
            return new f(
                function () {
                    c = e.getEnumerator();
                },
                function () {
                    if (c.moveNext()) {
                        b(c.current(), g++);
                        return this.yieldReturn(c.current());
                    }
                    return a;
                },
                function () {
                    d.dispose(c);
                }
            );
        });
    };
    c.prototype.forEach = function (c) {
        c = d.createLambda(c);
        var e = 0,
            b = this.getEnumerator();
        try {
            while (b.moveNext()) if (c(b.current(), e++) === a) break;
        } finally {
            d.dispose(b);
        }
    };
    c.prototype.write = function (c, f) {
        if (c == b) c = "";
        f = d.createLambda(f);
        var g = e;
        this.forEach(function (b) {
            if (g) g = a;
            else document.write(c);
            document.write(f(b));
        });
    };
    c.prototype.writeLine = function (a) {
        a = d.createLambda(a);
        this.forEach(function (b) {
            document.writeln(a(b) + "<br />");
        });
    };
    c.prototype.force = function () {
        var a = this.getEnumerator();
        try {
            while (a.moveNext());
        } finally {
            d.dispose(a);
        }
    };
    c.prototype.letBind = function (b) {
        b = d.createLambda(b);
        var e = this;
        return new c(function () {
            var g;
            return new f(
                function () {
                    g = c.from(b(e)).getEnumerator();
                },
                function () {
                    return g.moveNext() ? this.yieldReturn(g.current()) : a;
                },
                function () {
                    d.dispose(g);
                }
            );
        });
    };
    c.prototype.share = function () {
        var i = this,
            c,
            h = a;
        return new s(
            function () {
                return new f(
                    function () {
                        if (c == b) c = i.getEnumerator();
                    },
                    function () {
                        if (h) throw new Error(l);
                        return c.moveNext() ? this.yieldReturn(c.current()) : a;
                    },
                    g.Blank
                );
            },
            function () {
                h = e;
                d.dispose(c);
            }
        );
    };
    c.prototype.memoize = function () {
        var j = this,
            h,
            c,
            i = a;
        return new s(
            function () {
                var d = -1;
                return new f(
                    function () {
                        if (c == b) {
                            c = j.getEnumerator();
                            h = [];
                        }
                    },
                    function () {
                        if (i) throw new Error(l);
                        d++;
                        return h.length <= d ? (c.moveNext() ? this.yieldReturn((h[d] = c.current())) : a) : this.yieldReturn(h[d]);
                    },
                    g.Blank
                );
            },
            function () {
                i = e;
                d.dispose(c);
                h = b;
            }
        );
    };
    c.prototype.catchError = function (b) {
        b = d.createLambda(b);
        var e = this;
        return new c(function () {
            var c;
            return new f(
                function () {
                    c = e.getEnumerator();
                },
                function () {
                    try {
                        return c.moveNext() ? this.yieldReturn(c.current()) : a;
                    } catch (d) {
                        b(d);
                        return a;
                    }
                },
                function () {
                    d.dispose(c);
                }
            );
        });
    };
    c.prototype.finallyAction = function (b) {
        b = d.createLambda(b);
        var e = this;
        return new c(function () {
            var c;
            return new f(
                function () {
                    c = e.getEnumerator();
                },
                function () {
                    return c.moveNext() ? this.yieldReturn(c.current()) : a;
                },
                function () {
                    try {
                        d.dispose(c);
                    } finally {
                        b();
                    }
                }
            );
        });
    };
    c.prototype.log = function (a) {
        a = d.createLambda(a);
        return this.doAction(function (b) {
            typeof console !== i.Undefined && console.log(a(b));
        });
    };
    c.prototype.trace = function (c, a) {
        if (c == b) c = "Trace";
        a = d.createLambda(a);
        return this.doAction(function (b) {
            typeof console !== i.Undefined && console.log(c, a(b));
        });
    };
    var k = function (f, b, c, e) {
        var a = this;
        a.source = f;
        a.keySelector = d.createLambda(b);
        a.descending = c;
        a.parent = e;
    };
    k.prototype = new c();
    k.prototype.createOrderedEnumerable = function (a, b) {
        return new k(this.source, a, b, this);
    };
    k.prototype.thenBy = function (b) {
        return this.createOrderedEnumerable(b, a);
    };
    k.prototype.thenByDescending = function (a) {
        return this.createOrderedEnumerable(a, e);
    };
    k.prototype.getEnumerator = function () {
        var h = this,
            d,
            c,
            e = 0;
        return new f(
            function () {
                d = [];
                c = [];
                h.source.forEach(function (b, a) {
                    d.push(b);
                    c.push(a);
                });
                var a = p.create(h, b);
                a.GenerateKeys(d);
                c.sort(function (b, c) {
                    return a.compare(b, c);
                });
            },
            function () {
                return e < c.length ? this.yieldReturn(d[c[e++]]) : a;
            },
            g.Blank
        );
    };
    var p = function (c, d, e) {
        var a = this;
        a.keySelector = c;
        a.descending = d;
        a.child = e;
        a.keys = b;
    };
    p.create = function (a, d) {
        var c = new p(a.keySelector, a.descending, d);
        return a.parent != b ? p.create(a.parent, c) : c;
    };
    p.prototype.GenerateKeys = function (d) {
        var a = this;
        for (var f = d.length, g = a.keySelector, e = new Array(f), c = 0; c < f; c++) e[c] = g(d[c]);
        a.keys = e;
        a.child != b && a.child.GenerateKeys(d);
    };
    p.prototype.compare = function (e, f) {
        var a = this,
            c = d.compare(a.keys[e], a.keys[f]);
        return c == 0 ? (a.child != b ? a.child.compare(e, f) : d.compare(e, f)) : a.descending ? -c : c;
    };
    var s = function (a, b) {
        this.dispose = b;
        c.call(this, a);
    };
    s.prototype = new c();
    var h = function (a) {
        this.getSource = function () {
            return a;
        };
    };
    h.prototype = new c();
    h.prototype.any = function (a) {
        return a == b ? this.getSource().length > 0 : c.prototype.any.apply(this, arguments);
    };
    h.prototype.count = function (a) {
        return a == b ? this.getSource().length : c.prototype.count.apply(this, arguments);
    };
    h.prototype.elementAt = function (a) {
        var b = this.getSource();
        return 0 <= a && a < b.length ? b[a] : c.prototype.elementAt.apply(this, arguments);
    };
    h.prototype.elementAtOrDefault = function (c, a) {
        if (a === j) a = b;
        var d = this.getSource();
        return 0 <= c && c < d.length ? d[c] : a;
    };
    h.prototype.first = function (d) {
        var a = this.getSource();
        return d == b && a.length > 0 ? a[0] : c.prototype.first.apply(this, arguments);
    };
    h.prototype.firstOrDefault = function (e, a) {
        if (a === j) a = b;
        if (e != b) return c.prototype.firstOrDefault.apply(this, arguments);
        var d = this.getSource();
        return d.length > 0 ? d[0] : a;
    };
    h.prototype.last = function (d) {
        var a = this.getSource();
        return d == b && a.length > 0 ? a[a.length - 1] : c.prototype.last.apply(this, arguments);
    };
    h.prototype.lastOrDefault = function (e, a) {
        if (a === j) a = b;
        if (e != b) return c.prototype.lastOrDefault.apply(this, arguments);
        var d = this.getSource();
        return d.length > 0 ? d[d.length - 1] : a;
    };
    h.prototype.skip = function (d) {
        var b = this.getSource();
        return new c(function () {
            var c;
            return new f(
                function () {
                    c = d < 0 ? 0 : d;
                },
                function () {
                    return c < b.length ? this.yieldReturn(b[c++]) : a;
                },
                g.Blank
            );
        });
    };
    h.prototype.takeExceptLast = function (a) {
        if (a == b) a = 1;
        return this.take(this.getSource().length - a);
    };
    h.prototype.takeFromLast = function (a) {
        return this.skip(this.getSource().length - a);
    };
    h.prototype.reverse = function () {
        var b = this.getSource();
        return new c(function () {
            var c;
            return new f(
                function () {
                    c = b.length;
                },
                function () {
                    return c > 0 ? this.yieldReturn(b[--c]) : a;
                },
                g.Blank
            );
        });
    };
    h.prototype.sequenceEqual = function (d, e) {
        return (d instanceof h || d instanceof Array) && e == b && c.from(d).count() != this.count() ? a : c.prototype.sequenceEqual.apply(this, arguments);
    };
    h.prototype.toJoinedString = function (a, e) {
        var d = this.getSource();
        if (e != b || !(d instanceof Array)) return c.prototype.toJoinedString.apply(this, arguments);
        if (a == b) a = "";
        return d.join(a);
    };
    h.prototype.getEnumerator = function () {
        var a = this.getSource(),
            b = -1;
        return {
            current: function () {
                return a[b];
            },
            moveNext: function () {
                return ++b < a.length;
            },
            dispose: g.Blank,
        };
    };
    var n = function (b, a) {
        this.prevSource = b;
        this.prevPredicate = a;
    };
    n.prototype = new c();
    n.prototype.where = function (a) {
        a = d.createLambda(a);
        if (a.length <= 1) {
            var e = this.prevPredicate,
                b = function (b) {
                    return e(b) && a(b);
                };
            return new n(this.prevSource, b);
        } else return c.prototype.where.call(this, a);
    };
    n.prototype.select = function (a) {
        a = d.createLambda(a);
        return a.length <= 1 ? new m(this.prevSource, this.prevPredicate, a) : c.prototype.select.call(this, a);
    };
    n.prototype.getEnumerator = function () {
        var c = this.prevPredicate,
            e = this.prevSource,
            b;
        return new f(
            function () {
                b = e.getEnumerator();
            },
            function () {
                while (b.moveNext()) if (c(b.current())) return this.yieldReturn(b.current());
                return a;
            },
            function () {
                d.dispose(b);
            }
        );
    };
    var m = function (c, a, b) {
        this.prevSource = c;
        this.prevPredicate = a;
        this.prevSelector = b;
    };
    m.prototype = new c();
    m.prototype.where = function (a) {
        a = d.createLambda(a);
        return a.length <= 1 ? new n(this, a) : c.prototype.where.call(this, a);
    };
    m.prototype.select = function (a) {
        var b = this;
        a = d.createLambda(a);
        if (a.length <= 1) {
            var f = b.prevSelector,
                e = function (b) {
                    return a(f(b));
                };
            return new m(b.prevSource, b.prevPredicate, e);
        } else return c.prototype.select.call(b, a);
    };
    m.prototype.getEnumerator = function () {
        var e = this.prevPredicate,
            g = this.prevSelector,
            h = this.prevSource,
            c;
        return new f(
            function () {
                c = h.getEnumerator();
            },
            function () {
                while (c.moveNext()) if (e == b || e(c.current())) return this.yieldReturn(g(c.current()));
                return a;
            },
            function () {
                d.dispose(c);
            }
        );
    };
    var r = (function () {
            var d = function (a, b) {
                    return Object.prototype.hasOwnProperty.call(a, b);
                },
                h = function (a) {
                    return a === b ? "null" : a === j ? "undefined" : typeof a.toString === i.Function ? a.toString() : Object.prototype.toString.call(a);
                },
                m = function (d, c) {
                    var a = this;
                    a.key = d;
                    a.value = c;
                    a.prev = b;
                    a.next = b;
                },
                k = function () {
                    this.first = b;
                    this.last = b;
                };
            k.prototype = {
                addLast: function (c) {
                    var a = this;
                    if (a.last != b) {
                        a.last.next = c;
                        c.prev = a.last;
                        a.last = c;
                    } else a.first = a.last = c;
                },
                replace: function (c, a) {
                    if (c.prev != b) {
                        c.prev.next = a;
                        a.prev = c.prev;
                    } else this.first = a;
                    if (c.next != b) {
                        c.next.prev = a;
                        a.next = c.next;
                    } else this.last = a;
                },
                remove: function (a) {
                    if (a.prev != b) a.prev.next = a.next;
                    else this.first = a.next;
                    if (a.next != b) a.next.prev = a.prev;
                    else this.last = a.prev;
                },
            };
            var l = function (c) {
                var a = this;
                a.countField = 0;
                a.entryList = new k();
                a.buckets = {};
                a.compareSelector = c == b ? g.Identity : c;
            };
            l.prototype = {
                add: function (i, j) {
                    var a = this,
                        g = a.compareSelector(i),
                        f = h(g),
                        c = new m(i, j);
                    if (d(a.buckets, f)) {
                        for (var b = a.buckets[f], e = 0; e < b.length; e++)
                            if (a.compareSelector(b[e].key) === g) {
                                a.entryList.replace(b[e], c);
                                b[e] = c;
                                return;
                            }
                        b.push(c);
                    } else a.buckets[f] = [c];
                    a.countField++;
                    a.entryList.addLast(c);
                },
                get: function (i) {
                    var a = this,
                        c = a.compareSelector(i),
                        g = h(c);
                    if (!d(a.buckets, g)) return j;
                    for (var e = a.buckets[g], b = 0; b < e.length; b++) {
                        var f = e[b];
                        if (a.compareSelector(f.key) === c) return f.value;
                    }
                    return j;
                },
                set: function (k, l) {
                    var b = this,
                        g = b.compareSelector(k),
                        j = h(g);
                    if (d(b.buckets, j))
                        for (var f = b.buckets[j], c = 0; c < f.length; c++)
                            if (b.compareSelector(f[c].key) === g) {
                                var i = new m(k, l);
                                b.entryList.replace(f[c], i);
                                f[c] = i;
                                return e;
                            }
                    return a;
                },
                contains: function (j) {
                    var b = this,
                        f = b.compareSelector(j),
                        i = h(f);
                    if (!d(b.buckets, i)) return a;
                    for (var g = b.buckets[i], c = 0; c < g.length; c++) if (b.compareSelector(g[c].key) === f) return e;
                    return a;
                },
                clear: function () {
                    this.countField = 0;
                    this.buckets = {};
                    this.entryList = new k();
                },
                remove: function (g) {
                    var a = this,
                        f = a.compareSelector(g),
                        e = h(f);
                    if (!d(a.buckets, e)) return;
                    for (var b = a.buckets[e], c = 0; c < b.length; c++)
                        if (a.compareSelector(b[c].key) === f) {
                            a.entryList.remove(b[c]);
                            b.splice(c, 1);
                            if (b.length == 0) delete a.buckets[e];
                            a.countField--;
                            return;
                        }
                },
                count: function () {
                    return this.countField;
                },
                toEnumerable: function () {
                    var d = this;
                    return new c(function () {
                        var c;
                        return new f(
                            function () {
                                c = d.entryList.first;
                            },
                            function () {
                                if (c != b) {
                                    var d = { key: c.key, value: c.value };
                                    c = c.next;
                                    return this.yieldReturn(d);
                                }
                                return a;
                            },
                            g.Blank
                        );
                    });
                },
            };
            return l;
        })(),
        v = function (a) {
            var b = this;
            b.count = function () {
                return a.count();
            };
            b.get = function (b) {
                return c.from(a.get(b));
            };
            b.contains = function (b) {
                return a.contains(b);
            };
            b.toEnumerable = function () {
                return a.toEnumerable().select(function (a) {
                    return new t(a.key, a.value);
                });
            };
        },
        t = function (b, a) {
            this.key = function () {
                return b;
            };
            h.call(this, a);
        };
    t.prototype = new h();
    if (typeof define === i.Function && define.amd)
        define("linqjs", [], function () {
            return c;
        });
    else if (typeof module !== i.Undefined && module.exports) module.exports = c;
    else w.Enumerable = c;
})(this);
