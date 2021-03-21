<template>
    <div class="col-12">
        <slot v-if="!busy"></slot>
        <span v-if="busy">
            <div :style="getStyle">
                <vue-content-loading :width="getWidth" :height="getHeight">
                    <rect :x="getLineX(index)" :y="getLineY(index)" rx="4" ry="4" :width="width" :height="height" v-for="(n, index) in column" v-bind:key="index" />
                </vue-content-loading>
            </div>
        </span>
    </div>
</template>
<script>
import VueContentLoading from "vue-content-loading";

export default {
    components : {VueContentLoading},
    methods: {
        getLineX(index) {
            if (this.orientation == "horizontal") {
                return (index * this.width) + (index > 0 ? this.margin * index : 0)
            } 
            if (this.orientation == "vertical") {
                return 0;
            }
            return 0;
        },
        getLineY(index) {
            if (this.orientation == "horizontal") {
                return 0;
            } 
            if (this.orientation == "vertical") {
                return (index * this.height) + (index > 0 ? this.margin * index : 0)
            }
            return 0;
        }

    },
    computed:{
        getStyle() {
            var style = "";
            if (this.orientation == "horizontal") {
                style = "width:" + ((this.width * this.column) + (this.width * (this.column-1))) + "px;height:"+this.height+"px";
            } 
            if (this.orientation == "vertical") {
                style = "width:" + (this.width + this.height) + "px;height:"+((this.height * this.column) + (this.height * (this.column-1)))+"px";
            }
            return style;
        },
        getWidth() {
            if (this.orientation == "horizontal") {
                return (this.width * this.column) + (this.margin * (this.column-1));
            } 
            if (this.orientation == "vertical") {
                return this.width;
            }
            return 0;
        },
        getHeight() {
            if (this.orientation == "horizontal") {
                return this.height;
            } 
            if (this.orientation == "vertical") {
                return (this.height * this.column) + (this.margin * (this.column-1));
            }
            return 0;
        }
    },
    props : {
        width : {
            type:Number,
            default() {
                return 100;
            }
        },
        height : {
            type:Number,
            default() {
                return 15;
            }
        },
        margin : {
            type:Number,
            default() {
                return 10;
            }
        },
        column : {
            type: Number,
            default() {
                return 1;
            }
        },
        busy : {
            type: Boolean,
            default() {
                return false;
            }
        },
        orientation : {
            type: String,
            default() {
                return "horizontal";
            }
        }
    }

}
</script>