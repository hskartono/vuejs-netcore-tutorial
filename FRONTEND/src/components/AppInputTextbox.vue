<template>
    <div>
        <b-form-group id="fieldset-is-received" v-bind="$attrs">
            <template v-slot:label>{{ labelStr }} <span v-if="isRequired" class="required"></span></template>
            <div v-if="isAllowEdit">
                <b-input-group>
                    <b-input-group-prepend is-text v-if="prefixExist">
                        {{ prefix }}
                    </b-input-group-prepend>
                    <b-form-input v-bind="$attrs" v-model="inputModel" :formatter="inputFormatter" :lazy-formatter="true" @input="inputFocus" @blur="inputUpdate" :maxlength="maxLength" @keyup.enter="enterAction"></b-form-input>
                    <b-input-group-append is-text v-if="suffixExist">
                        {{ suffix }}
                    </b-input-group-append>
                </b-input-group>
                <b-form-invalid-feedback id="input-name-live-feedback" :state="state">
                    {{ errorMessage }}
                </b-form-invalid-feedback>
            </div>
            <div v-else>
                {{ inputModel }}
            </div>
        </b-form-group>
    </div>
</template>
<script>
export default {
    props: ["model", "errorMessage", "suffix", "prefix", "state", "maxLength", "thousandSeparator", "isEditable", "change", "label", "isRequired", "enterMethod"],
    data() {
        return {
            labelStr: this.label,
            inputModel:this.model,
            prefixExist: (this.prefix ? true : false),
            suffixExist: (this.suffix ? true : false),
        }
    }, 
    methods: {
        inputUpdate() {
            if (this.thousandSeparator) {
                this.inputModel = this.inputModel.replace(/,/g, "");
            }
            this.$emit('update:model', this.inputModel);
            this.$emit('change');
        },
        inputFocus() {
            if (this.thousandSeparator) {
                if (this.inputModel != null) {
                    this.inputModel = this.inputModel.replace(/,/g, "");
                }
            }
        },
        enterAction() {
            this.$emit('update:model', this.inputModel);
            this.$emit('enterMethod')
        },
        inputFormatter(value) {
            if (this.thousandSeparator) {
                return value
                .replace(/\D/g, "")
                .replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            }
            return value;
        }
    },
    computed : {
        isAllowEdit : {
            get: function() {
                if (this.isEditable != undefined) return this.isEditable;
                else return true;
            }
        }
    },
    watch : {
        model : function(val) {
            this.inputModel = val;
        },
    }
}
</script>
<style scoped>
.input-group-text {
    font-size: 0.7rem !important;
}
</style>