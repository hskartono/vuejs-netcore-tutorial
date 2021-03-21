<template>
    <div>
        <b-form-group :id="'fieldset-' + id" v-bind="$attrs">
            <b-row>
                <b-col>
                    <b-input-group>
                        <b-input-group-prepend is-text v-if="prefixExist">
                            {{ prefix }}
                        </b-input-group-prepend>
                        <b-form-datepicker v-bind="$attrs" :max="inputModelTo" v-model="inputModelFrom" :formatter="inputFormatter" @input="$emit('update:modelFrom', inputModelFrom);"></b-form-datepicker>
                        <b-input-group-append is-text v-if="suffixExist">
                            {{ suffix }}
                        </b-input-group-append>
                    </b-input-group>
                </b-col>
                <b-col>
                    <b-input-group>
                        <b-input-group-prepend is-text v-if="prefixExist">
                            {{ prefix }}
                        </b-input-group-prepend>
                        <b-form-datepicker v-bind="$attrs" :min="inputModelFrom" v-model="inputModelTo" :formatter="inputFormatter" @input="$emit('update:modelTo', inputModelTo);" :dropright="inputDateToDropupRight"></b-form-datepicker>
                        <b-input-group-append is-text v-if="suffixExist">
                            {{ suffix }}
                        </b-input-group-append>
                    </b-input-group>
                </b-col>
            </b-row>
            <b-form-invalid-feedback :id="'input-' + id + '-live-feedback'">
                {{ errorMessage }}
            </b-form-invalid-feedback>
        </b-form-group>
    </div>
</template>
<script>
export default {
    props: ["modelFrom", "modelTo", "errorMessage", "id", "suffix", "prefix", "isEditable", "dateToDropupRight"],
    data() {
        return {
            inputModelFrom: this.modelFrom,
            inputModelTo: this.modelTo,
            prefixExist: (this.prefix ? true : false),
            suffixExist: (this.suffix ? true : false),
            inputDateToDropupRight: (this.dateToDropupRight ? true : false)
        }
    }, 
    methods: {
        inputFormatter(value) {
            if (this.thousandSeparator) {
                return value
                .replace(/\D/g, "")
                .replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            }
            return value;
        }
    },
    watch: {
        modelFrom(newValue, oldValue) {
            if (newValue == "") {
                this.inputModelFrom = newValue;
            }
        },
        modelTo(newValue, oldValue) {
            if (newValue == "") {
                this.inputModelTo = newValue;
            }
        }
    }
}
</script>
<style scoped>
.input-group-text {
    font-size: 0.7rem !important;
}
</style>